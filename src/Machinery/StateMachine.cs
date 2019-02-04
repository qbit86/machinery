namespace Machinery
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;

    public static class StateMachine<TContext, TEvent>
    {
#pragma warning disable CA1000 // Do not declare static members on generic types
        public static StateMachine<TContext, TState, TEvent, TStatePolicy> Create<TState, TStatePolicy>(
            TState initialState, TStatePolicy statePolicy)
            where TStatePolicy : IStatePolicy<TContext, TState, TEvent>
        {
            return new StateMachine<TContext, TState, TEvent, TStatePolicy>(initialState, statePolicy);
        }
#pragma warning restore CA1000 // Do not declare static members on generic types
    }

    public sealed class StateMachine<TContext, TState, TEvent, TStatePolicy>
        where TStatePolicy : IStatePolicy<TContext, TState, TEvent>
    {
        private readonly TStatePolicy _statePolicy;

        private TState _currentState;
        private int _reentrantLockAcquired;

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
            int lockAlreadyAcquired = Interlocked.Exchange(ref _reentrantLockAcquired, 1);
            if (lockAlreadyAcquired == 1)
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
                Interlocked.Exchange(ref _reentrantLockAcquired, 0);
            }

            return true;
        }
    }
}
