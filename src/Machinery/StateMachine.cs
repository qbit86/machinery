namespace Machinery
{
    using System;
    using System.Threading;

    public static class StateMachine<TEvent>
    {
#pragma warning disable CA1000 // Do not declare static members on generic types
        public static StateMachine<TState, TEvent, TEventSink> Create<TState, TEventSink>(
            TState initialState, TEventSink eventSink)
            where TEventSink : IEventSink<TState, TEvent>
        {
            return new StateMachine<TState, TEvent, TEventSink>(initialState, eventSink);
        }
#pragma warning restore CA1000 // Do not declare static members on generic types
    }

#pragma warning disable CA1303 // Do not pass literals as localized parameters

    public sealed class StateMachine<TState, TEvent, TEventSink> : IStateMachine<TState, TEvent>
        where TEventSink : IEventSink<TState, TEvent>
    {
        private readonly TEventSink _eventSink;

        private TState _currentState;
        private int _lock;

        public StateMachine(TState initialState, TEventSink eventSink)
        {
            if (initialState is null)
                throw new ArgumentNullException(nameof(initialState));

            if (eventSink is null)
                throw new ArgumentNullException(nameof(eventSink));

            _currentState = initialState;
            _eventSink = eventSink;
        }

        public TState CurrentState => _currentState;

        public bool ProcessEvent(TEvent ev)
        {
            if (Interlocked.Exchange(ref _lock, 1) == 1)
                return false;

            try
            {
                bool transit = _eventSink.TryCreateNewState(_currentState, ev, out TState newState);
                if (!transit)
                    return true;

                if (newState is null)
                    throw new InvalidOperationException("The new state must not be null.");

                try
                {
                    _eventSink.OnExiting(_currentState, ev, newState);
                }
                catch
                {
                    _eventSink.DisposeState(newState);
                    throw;
                }

                TState oldState = _currentState;
                _currentState = newState;

                try
                {
                    _eventSink.OnEntered(_currentState, ev, oldState);
                }
                finally
                {
                    _eventSink.DisposeState(oldState);
                }
            }
            finally
            {
                Interlocked.Exchange(ref _lock, 0);
            }

            return true;
        }
    }
}
