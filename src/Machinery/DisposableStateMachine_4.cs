namespace Machinery
{
    using System;
    using System.Threading;

    public static partial class DisposableStateMachine<TEvent>
    {
#pragma warning disable CA1000 // Do not declare static members on generic types
        public static DisposableStateMachine<TContext, TEvent, TState, TPolicy> Create<TContext, TState, TPolicy>(
            TContext context, TState initialState, TPolicy policy)
            where TPolicy : IPolicy<TContext, TEvent, TState>
        {
            return new DisposableStateMachine<TContext, TEvent, TState, TPolicy>(context, initialState, policy);
        }
#pragma warning restore CA1000 // Do not declare static members on generic types
    }

    public sealed class DisposableStateMachine<TContext, TEvent, TState, TPolicy>
        where TPolicy : IPolicy<TContext, TEvent, TState>
    {
        private readonly TContext _context;
        private readonly TPolicy _policy;

        private TState _currentState;
        private int _lock;

        public DisposableStateMachine(TContext context, TState initialState, TPolicy policy)
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
            if (Interlocked.Exchange(ref _lock, 1) != 0)
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
            if (!transit || newState is null)
            {
                _policy.OnRemain(_context, ev, _currentState);
                return;
            }

            try
            {
                _policy.OnExiting(_context, ev, _currentState, newState);
            }
            catch
            {
                (newState as IDisposable)?.Dispose();
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
                (oldState as IDisposable)?.Dispose();
            }
        }
    }
}
