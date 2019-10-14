// ReSharper disable SuspiciousTypeConversion.Global

namespace Machinery
{
    using System;
    using System.Threading;

    public sealed class StateMachine<TContext, TEvent>
    {
        private readonly TContext _context;

        private IState<TContext, TEvent> _currentState;
        private int _lock;

        public StateMachine(TContext context, IState<TContext, TEvent> initialState)
        {
            if (context is null)
                throw new ArgumentNullException(nameof(context));

            _context = context;
            _currentState = initialState ?? throw new ArgumentNullException(nameof(initialState));
        }

        public IState<TContext, TEvent> CurrentState => _currentState;

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
            bool transit = _currentState.TryCreateNewState(_context, ev, out IState<TContext, TEvent> newState);
            if (!transit || newState is null)
            {
                _currentState.OnRemain(_context, ev);
                return;
            }

            try
            {
                _currentState.OnExiting(_context, ev, newState);
            }
            catch
            {
                (newState as IDisposable)?.Dispose();
                throw;
            }

            IState<TContext, TEvent> oldState = _currentState;
            _currentState = newState;

            try
            {
                _currentState.OnEntered(_context, ev, oldState);
            }
            finally
            {
                (oldState as IDisposable)?.Dispose();
            }
        }
    }
}
