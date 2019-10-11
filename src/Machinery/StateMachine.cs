namespace Machinery
{
    using System;
    using System.Threading;

    public static class StateMachine<TEvent>
    {
#pragma warning disable CA1000 // Do not declare static members on generic types
        public static StateMachine<TState, TEvent, TEventSink> Create<TState, TEventSink>(
            TState initialState, TEventSink eventSink)
            where TEventSink : IPolicy<TState, TEvent>
        {
            return new StateMachine<TState, TEvent, TEventSink>(initialState, eventSink);
        }
#pragma warning restore CA1000 // Do not declare static members on generic types
    }

#pragma warning disable CA1303 // Do not pass literals as localized parameters

    public sealed class StateMachine<TState, TEvent, TPolicy>
        where TPolicy : IPolicy<TState, TEvent>
    {
        private readonly TPolicy _policy;

        private TState _currentState;
        private int _lock;

        public StateMachine(TState initialState, TPolicy policy)
        {
            if (initialState is null)
                throw new ArgumentNullException(nameof(initialState));

            if (policy is null)
                throw new ArgumentNullException(nameof(policy));

            _currentState = initialState;
            _policy = policy;
        }

        public TState CurrentState => _currentState;

        public bool ProcessEvent(TEvent ev)
        {
            if (Interlocked.Exchange(ref _lock, 1) == 1)
                return false;

            try
            {
                bool transit = _policy.TryCreateNewState(_currentState, ev, out TState newState);
                if (!transit)
                    return true;

                if (newState is null)
                    throw new InvalidOperationException("The new state must not be null.");

                try
                {
                    _policy.OnExiting(_currentState, ev, newState);
                }
                catch
                {
                    _policy.DisposeState(newState);
                    throw;
                }

                TState oldState = _currentState;
                _currentState = newState;

                try
                {
                    _policy.OnEntered(_currentState, ev, oldState);
                }
                finally
                {
                    _policy.DisposeState(oldState);
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
