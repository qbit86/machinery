namespace Machinery
{
    using System;
    using System.Threading;

    /// <summary>
    /// Provides the factory method for <see cref="DisposableStateMachine{TContext,TEvent,TState}" />.
    /// </summary>
    /// <typeparam name="TEvent">The type of the events.</typeparam>
    public static class DisposableStateMachine<TEvent>
    {
        /// <summary>
        /// Creates a new <see cref="DisposableStateMachine{TContext,TEvent,TState}" /> from the specified context and initial state.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="initialState">The initial state.</param>
        /// <typeparam name="TContext">The type of the context.</typeparam>
        /// <typeparam name="TState">The type of the states.</typeparam>
        /// <returns>A <see cref="DisposableStateMachine{TContext,TEvent,TState}" /> in the initial state.</returns>
        public static DisposableStateMachine<TContext, TEvent, TState> Create<TContext, TState>(
            TContext context, TState initialState)
            where TState : IState<TContext, TEvent, TState>, IDisposable =>
            new(context, initialState);
    }

    /// <summary>
    /// Represents a state machine that manages transitions between disposable states based on events.
    /// </summary>
    /// <typeparam name="TContext">The type of the context maintained by the state machine.</typeparam>
    /// <typeparam name="TEvent">The type of events that trigger state transitions.</typeparam>
    /// <typeparam name="TState">The type of states in the state machine.</typeparam>
    public sealed class DisposableStateMachine<TContext, TEvent, TState> : IDisposable
        where TState : IState<TContext, TEvent, TState>, IDisposable
    {
        private TState _currentState;
        private int _lock;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisposableStateMachine{TContext, TEvent, TState}" /> class with the specified context and initial state.
        /// </summary>
        /// <param name="context">The context for the state machine.</param>
        /// <param name="initialState">The initial state of the state machine.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="initialState" /> is null.</exception>
        public DisposableStateMachine(TContext context, TState initialState)
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
        /// <exception cref="ObjectDisposedException">Thrown when the state machine has been disposed.</exception>
        public TState CurrentState
        {
            get
            {
                if (_lock == -1)
                    throw new ObjectDisposedException(nameof(DisposableStateMachine<TContext, TEvent, TState>));

                return _currentState;
            }
        }

        /// <summary>
        /// Disposes the current state and releases all resources used by the state machine.
        /// </summary>
        public void Dispose()
        {
            if (_lock == -1)
                return;

            var currentState = _currentState;
            _currentState = default!;
            currentState.Dispose();

            _lock = -1;
        }

        /// <summary>
        /// Attempts to process an event, acquiring a lock first to prevent reentrancy.
        /// </summary>
        /// <param name="ev">The event to process.</param>
        /// <returns>True if the event was processed; otherwise, false (if the state machine is already processing another event).</returns>
        /// <exception cref="ObjectDisposedException">Thrown when the state machine has been disposed.</exception>
        public bool TryProcessEvent(TEvent ev)
        {
            if (_lock == -1)
                throw new ObjectDisposedException(nameof(DisposableStateMachine<TContext, TEvent, TState>));

            if (Interlocked.Exchange(ref _lock, 1) != 0)
                return false;

            try
            {
                TryProcessEventUnchecked(ev);
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
        /// A <see cref="ProcessingResult" /> indicating the outcome of the event processing:
        /// NotProcessed - The state machine is already processing another event.
        /// Remained - The event was processed but the state didn't change.
        /// Transitioned - The event was processed and the state changed.
        /// </returns>
        /// <exception cref="ObjectDisposedException">Thrown when the state machine has been disposed.</exception>
        public ProcessingResult ProcessEvent(TEvent ev)
        {
            if (_lock == -1)
                throw new ObjectDisposedException(nameof(DisposableStateMachine<TContext, TEvent, TState>));

            if (Interlocked.Exchange(ref _lock, 1) != 0)
                return ProcessingResult.NotProcessed;

            try
            {
                bool transitioned = TryProcessEventUnchecked(ev);
                return transitioned ? ProcessingResult.Transitioned : ProcessingResult.Remained;
            }
            finally
            {
                _lock = 0;
            }
        }

        private bool TryProcessEventUnchecked(TEvent ev)
        {
            bool transit = _currentState.TryCreateNewState(Context, ev, out var newState);
            if (!transit || newState is null)
            {
                _currentState.OnRemain(Context, ev);
                return false;
            }

            try
            {
                _currentState.OnExiting(Context, ev, newState);
                newState.OnEntering(Context, ev, _currentState);
            }
            catch
            {
                newState.Dispose();
                throw;
            }

            var oldState = _currentState;
            _currentState = newState;

            try
            {
                oldState.OnExited(Context, ev, _currentState);
                _currentState.OnEntered(Context, ev, oldState);
            }
            finally
            {
                oldState.Dispose();
            }

            return true;
        }
    }
}
