namespace Machinery
{
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the interface for a state in an asynchronous state machine.
    /// </summary>
    /// <typeparam name="TContext">The type of the context maintained by the state machine.</typeparam>
    /// <typeparam name="TEvent">The type of events that trigger state transitions.</typeparam>
    /// <typeparam name="TState">The type of states in the state machine.</typeparam>
    public interface IAsyncState<in TContext, in TEvent, TState>
    {
        /// <summary>
        /// Tries to create a new state based on the current context and event.
        /// </summary>
        /// <param name="context">The context of the state machine.</param>
        /// <param name="ev">The event triggering the potential state transition.</param>
        /// <param name="newState">When this method returns, contains the new state if a transition should occur, or default value if no transition should occur.</param>
        /// <returns>true if a transition to a new state should occur; otherwise, false.</returns>
        bool TryCreateNewState(TContext context, TEvent ev, [MaybeNullWhen(false)] out TState newState);

        /// <summary>
        /// Called asynchronously when the state machine is about to exit this state during a transition.
        /// </summary>
        /// <param name="context">The context of the state machine.</param>
        /// <param name="ev">The event that triggered the transition.</param>
        /// <param name="newState">The state that the state machine is transitioning to.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task OnExitingAsync(TContext context, TEvent ev, TState newState);

        /// <summary>
        /// Called asynchronously after the state machine has exited this state during a transition.
        /// </summary>
        /// <param name="context">The context of the state machine.</param>
        /// <param name="ev">The event that triggered the transition.</param>
        /// <param name="newState">The state that the state machine has transitioned to.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task OnExitedAsync(TContext context, TEvent ev, TState newState);

        /// <summary>
        /// Called asynchronously when the state machine remains in this state after processing an event.
        /// </summary>
        /// <param name="context">The context of the state machine.</param>
        /// <param name="ev">The event that was processed.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task OnRemainAsync(TContext context, TEvent ev);

        /// <summary>
        /// Called asynchronously when the state machine is about to enter this state during a transition.
        /// </summary>
        /// <param name="context">The context of the state machine.</param>
        /// <param name="ev">The event that triggered the transition.</param>
        /// <param name="oldState">The state that the state machine is transitioning from.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task OnEnteringAsync(TContext context, TEvent ev, TState oldState);

        /// <summary>
        /// Called asynchronously after the state machine has entered this state during a transition.
        /// </summary>
        /// <param name="context">The context of the state machine.</param>
        /// <param name="ev">The event that triggered the transition.</param>
        /// <param name="oldState">The state that the state machine has transitioned from.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task OnEnteredAsync(TContext context, TEvent ev, TState oldState);
    }
}
