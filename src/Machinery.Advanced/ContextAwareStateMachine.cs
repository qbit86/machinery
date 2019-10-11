namespace Machinery
{
    using System;
    using System.Threading;

    public static class ContextAwareStateMachine<TEvent, TContext>
    {
#pragma warning disable CA1000 // Do not declare static members on generic types
        public static ContextAwareStateMachine<TState, TEvent, TEventSink, TContext> Create<TState, TEventSink>(
            TState initialState, TEventSink eventSink)
            where TEventSink : IContextAwareEventSink<TState, TEvent, TContext>
        {
            return new ContextAwareStateMachine<TState, TEvent, TEventSink, TContext>(initialState, eventSink);
        }
#pragma warning restore CA1000 // Do not declare static members on generic types
    }

    public sealed class ContextAwareStateMachine<TState, TEvent, TEventSink, TContext> :
        IContextAwareStateMachine<TState, TEvent, TContext>
        where TEventSink : IContextAwareEventSink<TState, TEvent, TContext>
    {
        private readonly TEventSink _eventSink;

        private TState _currentState;
        private int _lock;

        public ContextAwareStateMachine(TState initialState, TEventSink eventSink)
        {
            if (initialState is null)
                throw new ArgumentNullException(nameof(initialState));

            if (eventSink is null)
                throw new ArgumentNullException(nameof(eventSink));

            _currentState = initialState;
            _eventSink = eventSink;
        }

        public TState CurrentState => _currentState;

        public bool ProcessEvent(TContext context, TEvent ev)
        {
            if (Interlocked.Exchange(ref _lock, 1) == 1)
                return false;

            try
            {
                bool transit = _eventSink.TryCreateNewState(context, _currentState, ev, out TState newState);
                if (!transit)
                    return true;

                if (newState is null)
                    throw new InvalidOperationException(nameof(newState));

                try
                {
                    _eventSink.OnExiting(context, _currentState, ev, newState);
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
                    _eventSink.OnEntered(context, _currentState, ev, oldState);
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
