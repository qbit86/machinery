namespace Machinery
{
    using System;
    using System.Threading;

    public static class StateMachine<TContext, TEvent>
    {
#pragma warning disable CA1000 // Do not declare static members on generic types
        public static StateMachine<TState, TEvent, TStatePolicy, TContext> Create<TState, TStatePolicy>(
            TState initialState, TStatePolicy statePolicy)
            where TStatePolicy : IStatePolicy<TState, TEvent, TContext>
        {
            return new StateMachine<TState, TEvent, TStatePolicy, TContext>(initialState, statePolicy);
        }
#pragma warning restore CA1000 // Do not declare static members on generic types
    }

    public sealed class StateMachine<TState, TEvent, TStatePolicy, TContext> : ICurrentStateProvider<TState>
        where TStatePolicy : IStatePolicy<TState, TEvent, TContext>
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

        public bool Process(TContext context, TEvent ev)
        {
            if (Interlocked.Exchange(ref _lock, 1) == 1)
                return false;

            try
            {
                bool transit = _statePolicy.TryCreateNewState(context, _currentState, ev, out TState newState);
                if (!transit)
                    return true;

                if (newState == null)
                    throw new InvalidOperationException(nameof(newState));

                try
                {
                    _statePolicy.OnExiting(context, _currentState, ev, newState);
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
                    _statePolicy.OnEntered(context, _currentState, ev, oldState);
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
