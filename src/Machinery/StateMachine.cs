namespace Machinery
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;

    public static class StateMachine<TContext, TEvent>
    {
        [SuppressMessage("Design", "CA1000")]
        public static StateMachine<TContext, TState, TEvent, TStatePolicy> Create<TState, TStatePolicy>(
            TState initialState, TStatePolicy statePolicy)
            where TStatePolicy : IStatePolicy<TContext, TState, TEvent>
        {
            return new StateMachine<TContext, TState, TEvent, TStatePolicy>(initialState, statePolicy);
        }
    }

    public sealed class StateMachine<TContext, TState, TEvent, TStatePolicy>
        where TStatePolicy : IStatePolicy<TContext, TState, TEvent>
    {
        private readonly TStatePolicy _statePolicy;

        private TState _currentState;
        private int _locked;

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
            int originalLocked = Interlocked.Exchange(ref _locked, 1);
            if (originalLocked == 1)
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
                    _statePolicy.DisposeState(context, _currentState, ev, newState);
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
                    _statePolicy.DisposeState(context, _currentState, ev, oldState);
                }
            }
            finally
            {
                Interlocked.Exchange(ref _locked, 0);
            }

            return true;
        }
    }
}
