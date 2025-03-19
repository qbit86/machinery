namespace Machinery
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides the factory method for <see cref="AsyncStateMachine{TContext,TEvent,TState}" />.
    /// </summary>
    /// <typeparam name="TEvent">The type of the events.</typeparam>
    public static class AsyncStateMachine<TEvent>
    {
        /// <summary>
        /// Creates a new <see cref="AsyncStateMachine{TContext,TEvent,TState}" /> from the specified context and initial state.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="initialState">The initial state.</param>
        /// <typeparam name="TContext">The type of the context.</typeparam>
        /// <typeparam name="TState">The type of the states.</typeparam>
        /// <returns>An <see cref="AsyncStateMachine{TContext,TEvent,TState}" /> in the initial state.</returns>
        public static AsyncStateMachine<TContext, TEvent, TState> Create<TContext, TState>(
            TContext context, TState initialState)
            where TState : IAsyncState<TContext, TEvent, TState> =>
            new(context, initialState);
    }

    /// <summary>
    /// Represents an asynchronous state machine that manages transitions between states based on events.
    /// </summary>
    /// <typeparam name="TContext">The type of the context maintained by the state machine.</typeparam>
    /// <typeparam name="TEvent">The type of events that trigger state transitions.</typeparam>
    /// <typeparam name="TState">The type of states in the state machine.</typeparam>
    public sealed class AsyncStateMachine<TContext, TEvent, TState>
        where TState : IAsyncState<TContext, TEvent, TState>
    {
        private TState _currentState;
        private int _lock;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncStateMachine{TContext, TEvent, TState}" /> class with the specified context and initial state.
        /// </summary>
        /// <param name="context">The context for the state machine.</param>
        /// <param name="initialState">The initial state of the state machine.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="initialState" /> is null.</exception>
        public AsyncStateMachine(TContext context, TState initialState)
        {
            if (initialState is null)
                throw new ArgumentNullException(nameof(initialState));

            Context = context;
            _currentState = initialState;
        }

        /// <summary>
        /// Gets the context of the state machine.
        /// </summary>
        public TContext Context { get; }

        /// <summary>
        /// Gets the current state of the state machine.
        /// </summary>
        public TState CurrentState => _currentState;

        /// <summary>
        /// Attempts to process an event, acquiring a lock first to prevent reentrancy.
        /// </summary>
        /// <param name="ev">The event to process.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result is true if the event was processed;
        /// otherwise, false (if the state machine is already processing another event).
        /// </returns>
        public async Task<bool> TryProcessEvent(TEvent ev)
        {
            if (Interlocked.Exchange(ref _lock, 1) != 0)
                return false;

            try
            {
                await TryUncheckedProcessEventAsync(ev).ConfigureAwait(false);
            }
            finally
            {
                _lock = 0;
            }

            return true;
        }

        /// <summary>
        /// Processes an event and returns a detailed result about what happened.
        /// </summary>
        /// <param name="ev">The event to process.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result is a <see cref="ProcessingResult" /> indicating the outcome of the event processing:
        /// NotProcessed - The state machine is already processing another event.
        /// Remained - The event was processed but the state didn't change.
        /// Transitioned - The event was processed and the state changed.
        /// </returns>
        public async Task<ProcessingResult> ProcessEventAsync(TEvent ev)
        {
            if (Interlocked.Exchange(ref _lock, 1) != 0)
                return ProcessingResult.NotProcessed;

            try
            {
                bool transitioned = await TryUncheckedProcessEventAsync(ev).ConfigureAwait(false);
                return transitioned ? ProcessingResult.Transitioned : ProcessingResult.Remained;
            }
            finally
            {
                _lock = 0;
            }
        }

        private async Task<bool> TryUncheckedProcessEventAsync(TEvent ev)
        {
            bool transit = _currentState.TryCreateNewState(Context, ev, out var newState);
            if (!transit || newState is null)
            {
                await _currentState.OnRemainAsync(Context, ev).ConfigureAwait(false);
                return false;
            }

            await _currentState.OnExitingAsync(Context, ev, newState).ConfigureAwait(false);
            await newState.OnEnteringAsync(Context, ev, _currentState).ConfigureAwait(false);

            var oldState = _currentState;
            _currentState = newState;

            await oldState.OnExitedAsync(Context, ev, _currentState).ConfigureAwait(false);
            await _currentState.OnEnteredAsync(Context, ev, oldState).ConfigureAwait(false);

            return true;
        }
    }
}
