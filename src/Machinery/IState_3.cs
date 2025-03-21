namespace Machinery
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Defines the interface for a state in a state machine.
    /// </summary>
    /// <typeparam name="TContext">The type of the context maintained by the state machine.</typeparam>
    /// <typeparam name="TEvent">The type of events that trigger state transitions.</typeparam>
    /// <typeparam name="TState">The type of states in the state machine.</typeparam>
    public interface IState<in TContext, in TEvent, TState>
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
        /// Called when the state machine is about to exit this state during a transition.
        /// </summary>
        /// <param name="context">The context of the state machine.</param>
        /// <param name="ev">The event that triggered the transition.</param>
        /// <param name="newState">The state that the state machine is transitioning to.</param>
        void OnExiting(TContext context, TEvent ev, TState newState);

        /// <summary>
        /// Called after the state machine has exited this state during a transition.
        /// </summary>
        /// <param name="context">The context of the state machine.</param>
        /// <param name="ev">The event that triggered the transition.</param>
        /// <param name="newState">The state that the state machine has transitioned to.</param>
        void OnExited(TContext context, TEvent ev, TState newState);

        /// <summary>
        /// Called when the state machine remains in this state after processing an event.
        /// </summary>
        /// <param name="context">The context of the state machine.</param>
        /// <param name="ev">The event that was processed.</param>
        void OnRemain(TContext context, TEvent ev);

        /// <summary>
        /// Called when the state machine is about to enter this state during a transition.
        /// </summary>
        /// <param name="context">The context of the state machine.</param>
        /// <param name="ev">The event that triggered the transition.</param>
        /// <param name="oldState">The state that the state machine is transitioning from.</param>
        void OnEntering(TContext context, TEvent ev, TState oldState);

        /// <summary>
        /// Called after the state machine has entered this state during a transition.
        /// </summary>
        /// <param name="context">The context of the state machine.</param>
        /// <param name="ev">The event that triggered the transition.</param>
        /// <param name="oldState">The state that the state machine has transitioned from.</param>
        void OnEntered(TContext context, TEvent ev, TState oldState);
    }
}
