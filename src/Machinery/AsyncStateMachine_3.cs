namespace Machinery
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides the factory method for <see cref="AsyncStateMachine{TContext,TEvent,TState}"/>.
    /// </summary>
    /// <typeparam name="TEvent">The type of the events.</typeparam>
    public static class AsyncStateMachine<TEvent>
    {
        /// <summary>
        /// Creates a new <see cref="AsyncStateMachine{TContext,TEvent,TState}"/> from the specified context and initial state.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="initialState">The initial state.</param>
        /// <typeparam name="TContext">The type of the context.</typeparam>
        /// <typeparam name="TState">The type of the states.</typeparam>
        /// <returns>An <see cref="AsyncStateMachine{TContext,TEvent,TState}"/> in the initial state.</returns>
        public static AsyncStateMachine<TContext, TEvent, TState> Create<TContext, TState>(
            TContext context, TState initialState)
            where TState : IAsyncState<TContext, TEvent, TState> =>
            new(context, initialState);
    }

    public sealed class AsyncStateMachine<TContext, TEvent, TState>
        where TState : IAsyncState<TContext, TEvent, TState>
    {
        private TState _currentState;
        private int _lock;

        public AsyncStateMachine(TContext context, TState initialState)
        {
            if (initialState is null)
                throw new ArgumentNullException(nameof(initialState));

            Context = context;
            _currentState = initialState;
        }

        public TContext Context { get; }

        public TState CurrentState => _currentState;

        public async Task<bool> TryProcessEvent(TEvent ev)
        {
            if (Interlocked.Exchange(ref _lock, 1) != 0)
                return false;

            try
            {
                await UncheckedProcessEventAsync(ev).ConfigureAwait(false);
            }
            finally
            {
                _lock = 0;
            }

            return true;
        }

        private async Task UncheckedProcessEventAsync(TEvent ev)
        {
            bool transit = _currentState.TryCreateNewState(Context, ev, out TState? newState);
            if (!transit || newState is null)
            {
                await _currentState.OnRemainAsync(Context, ev).ConfigureAwait(false);
                return;
            }

            await _currentState.OnExitingAsync(Context, ev, newState).ConfigureAwait(false);
            await newState.OnEnteringAsync(Context, ev, _currentState).ConfigureAwait(false);

            TState oldState = _currentState;
            _currentState = newState;

            await oldState.OnExitedAsync(Context, ev, _currentState).ConfigureAwait(false);
            await _currentState.OnEnteredAsync(Context, ev, oldState).ConfigureAwait(false);
        }
    }
}
