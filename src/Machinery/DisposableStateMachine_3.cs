namespace Machinery
{
    using System;
    using System.Threading;

    public static partial class DisposableStateMachine<TEvent>
    {
        public static DisposableStateMachine<TContext, TEvent, TState> Create<TContext, TState>(
            TContext context, TState initialState)
            where TState : IState<TContext, TEvent, TState>, IDisposable =>
            new(context, initialState);
    }

    public sealed class DisposableStateMachine<TContext, TEvent, TState> : IDisposable
        where TState : IState<TContext, TEvent, TState>, IDisposable
    {
        private readonly TContext _context;

        private TState _currentState;
        private int _lock;

        public DisposableStateMachine(TContext context, TState initialState)
        {
            if (initialState is null)
                throw new ArgumentNullException(nameof(initialState));

            _context = context;
            _currentState = initialState;
        }

        public TState CurrentState
        {
            get
            {
                if (_lock == -1)
                    throw new ObjectDisposedException(nameof(DisposableStateMachine<TContext, TEvent, TState>));

                return _currentState;
            }
        }

        public void Dispose()
        {
            if (_lock == -1)
                return;

            TState currentState = _currentState;
            _currentState = default!;
            currentState.Dispose();

            _lock = -1;
        }

        public bool TryProcessEvent(TEvent ev)
        {
            if (_lock == -1)
                throw new ObjectDisposedException(nameof(DisposableStateMachine<TContext, TEvent, TState>));

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
            bool transit = _currentState.TryCreateNewState(_context, ev, out TState? newState);
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
                newState.Dispose();
                throw;
            }

            TState oldState = _currentState;
            _currentState = newState;

            try
            {
                _currentState.OnEntered(_context, ev, oldState);
            }
            finally
            {
                oldState.Dispose();
            }
        }
    }
}
