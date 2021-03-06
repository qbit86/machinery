namespace Machinery
{
    using System;
    using System.Threading;

    public static class StateMachine<TEvent>
    {
        public static StateMachine<TContext, TEvent, TState> Create<TContext, TState>(
            TContext context, TState initialState)
            where TState : IState<TContext, TEvent, TState> =>
            new StateMachine<TContext, TEvent, TState>(context, initialState);
    }

    public sealed class StateMachine<TContext, TEvent, TState>
        where TState : IState<TContext, TEvent, TState>
    {
        private readonly TContext _context;

        private TState _currentState;
        private int _lock;

        public StateMachine(TContext context, TState initialState)
        {
            if (initialState is null)
                throw new ArgumentNullException(nameof(initialState));

            _context = context;
            _currentState = initialState;
        }

        public TContext Context => _context;

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
                _lock = 0;
            }

            return true;
        }

        private void UncheckedProcessEvent(TEvent ev)
        {
            bool transit = _currentState.TryCreateNewState(_context, ev, out TState newState);
            if (!transit || newState is null)
            {
                _currentState.OnRemain(_context, ev);
                return;
            }

            _currentState.OnExiting(_context, ev, newState);

            TState oldState = _currentState;
            _currentState = newState;

            _currentState.OnEntered(_context, ev, oldState);
        }
    }
}
