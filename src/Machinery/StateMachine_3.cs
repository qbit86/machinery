namespace Machinery
{
    using System;
    using System.Threading;

    /// <summary>
    /// Provides the factory method for <see cref="StateMachine{TContext,TEvent,TState}"/>.
    /// </summary>
    /// <typeparam name="TEvent">The type of the events.</typeparam>
    public static class StateMachine<TEvent>
    {
        /// <summary>
        /// Creates a new <see cref="StateMachine{TContext,TEvent,TState}"/> from the specified context and initial state.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="initialState">The initial state.</param>
        /// <typeparam name="TContext">The type of the context.</typeparam>
        /// <typeparam name="TState">The type of the states.</typeparam>
        /// <returns>A <see cref="StateMachine{TContext,TEvent,TState}"/> in the initial state.</returns>
        public static StateMachine<TContext, TEvent, TState> Create<TContext, TState>(
            TContext context, TState initialState)
            where TState : IState<TContext, TEvent, TState> =>
            new(context, initialState);
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
                ProcessEventUnchecked(ev);
            }
            finally
            {
                _lock = 0;
            }

            return true;
        }

        private void ProcessEventUnchecked(TEvent ev)
        {
            bool transit = _currentState.TryCreateNewState(_context, ev, out TState? newState);
            if (!transit || newState is null)
            {
                _currentState.OnRemain(_context, ev);
                return;
            }

            _currentState.OnExiting(_context, ev, newState);
            newState.OnEntering(_context, ev, _currentState);

            TState oldState = _currentState;
            _currentState = newState;

            oldState.OnExited(_context, ev, _currentState);
            _currentState.OnEntered(_context, ev, oldState);
        }
    }
}
