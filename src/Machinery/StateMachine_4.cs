namespace Machinery
{
    using System;
    using System.Threading;

    public static partial class StateMachine<TEvent>
    {
#pragma warning disable CA1000 // Do not declare static members on generic types
        public static StateMachine<TContext, TEvent, TState, TPolicy> Create<TContext, TState, TPolicy>(
            TContext context, TState initialState, TPolicy policy)
            where TPolicy : IPolicy<TContext, TEvent, TState>
        {
            return new StateMachine<TContext, TEvent, TState, TPolicy>(context, initialState, policy);
        }
#pragma warning restore CA1000 // Do not declare static members on generic types
    }

#pragma warning disable CA1303 // Do not pass literals as localized parameters

    public sealed class StateMachine<TContext, TEvent, TState, TPolicy>
        where TPolicy : IPolicy<TContext, TEvent, TState>
    {
        private readonly TContext _context;
        private readonly TPolicy _policy;

        private TState _currentState;
        private int _lock;

        public StateMachine(TContext context, TState initialState, TPolicy policy)
        {
            if (context is null)
                throw new ArgumentNullException(nameof(context));

            if (initialState is null)
                throw new ArgumentNullException(nameof(initialState));

            if (policy is null)
                throw new ArgumentNullException(nameof(policy));

            _context = context;
            _currentState = initialState;
            _policy = policy;
        }

        public TState CurrentState => _currentState;

        public bool TryProcessEvent(TEvent ev)
        {
            if (Interlocked.Exchange(ref _lock, 1) == 1)
                return false;

            try
            {
                UncheckedProcessEvent(ev);
            }
            finally
            {
                Interlocked.Exchange(ref _lock, 0);
            }

            return true;
        }

        private void UncheckedProcessEvent(TEvent ev)
        {
            bool transit = _policy.TryCreateNewState(_context, ev, _currentState, out TState newState);
            if (!transit)
                return;

            if (newState is null)
                throw new InvalidOperationException("The new state must not be null.");

            try
            {
                _policy.OnExiting(_context, ev, _currentState, newState);
            }
            catch
            {
                _policy.DisposeState(_context, ev, newState);
                throw;
            }

            TState oldState = _currentState;
            _currentState = newState;

            try
            {
                _policy.OnEntered(_context, ev, _currentState, oldState);
            }
            finally
            {
                _policy.DisposeState(_context, ev, oldState);
            }
        }
    }
}