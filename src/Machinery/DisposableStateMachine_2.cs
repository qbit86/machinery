// ReSharper disable SuspiciousTypeConversion.Global

namespace Machinery
{
    using System;
    using System.Threading;

    public sealed class DisposableStateMachine<TContext, TEvent> : IDisposable
    {
        private readonly TContext _context;

        private IState<TContext, TEvent> _currentState;
        private int _lock;

        public DisposableStateMachine(TContext context, IState<TContext, TEvent> initialState)
        {
            if (context is null)
                throw new ArgumentNullException(nameof(context));

            _context = context;
            _currentState = initialState ?? throw new ArgumentNullException(nameof(initialState));
        }

        public void Dispose()
        {
            if (_lock == -1)
                return;

            IState<TContext, TEvent> currentState = Interlocked.Exchange(ref _currentState, null);
            (currentState as IDisposable)?.Dispose();

            _lock = -1;
        }

        public bool TryGetCurrentState(out IState<TContext, TEvent> currentState)
        {
            if (_lock != 0)
            {
                currentState = default;
                return false;
            }

            currentState = _currentState;
            return true;
        }

        public bool TryProcessEvent(TEvent ev)
        {
            if (_lock == -1)
                return false;

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

            IState<TContext, TEvent> oldState = Interlocked.Exchange(ref _currentState, newState);

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
