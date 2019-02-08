namespace Machinery
{
    using System;
    using System.Threading;

    public static class StateMachine<TEvent>
    {
#pragma warning disable CA1000 // Do not declare static members on generic types
        public static StateMachine<TState, TEvent, TStatePolicy> Create<TState, TStatePolicy>(
            TState initialState, TStatePolicy statePolicy)
            where TStatePolicy : IStatePolicy<TState, TEvent>
        {
            return new StateMachine<TState, TEvent, TStatePolicy>(initialState, statePolicy);
        }
#pragma warning restore CA1000 // Do not declare static members on generic types
    }

    public sealed class StateMachine<TState, TEvent, TStatePolicy> : IStateMachine<TState, TEvent>
        where TStatePolicy : IStatePolicy<TState, TEvent>
    {
        private readonly TStatePolicy _statePolicy;

        private TState _currentState;
        private int _lock;

        public StateMachine(TState initialState, TStatePolicy statePolicy)
        {
            if (initialState == null)
                throw new ArgumentNullException(nameof(initialState));

            if (statePolicy == null)
                throw new ArgumentNullException(nameof(statePolicy));

            _currentState = initialState;
            _statePolicy = statePolicy;
        }

        public TState CurrentState => _currentState;

        public bool ProcessEvent(TEvent ev)
        {
            if (Interlocked.Exchange(ref _lock, 1) == 1)
                return false;

            try
            {
                bool transit = _statePolicy.TryCreateNewState(_currentState, ev, out TState newState);
                if (!transit)
                    return true;

                if (newState == null)
                    throw new InvalidOperationException(nameof(newState));

                try
                {
                    _statePolicy.OnExiting(_currentState, ev, newState);
                }
                catch
                {
                    _statePolicy.DisposeState(newState);
                    throw;
                }

                TState oldState = _currentState;
                _currentState = newState;

                try
                {
                    _statePolicy.OnEntered(_currentState, ev, oldState);
                }
                finally
                {
                    _statePolicy.DisposeState(oldState);
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
