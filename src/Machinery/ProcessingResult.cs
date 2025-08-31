namespace Machinery
{
    /// <summary>
    /// Represents the result of processing an event.
    /// </summary>
    public enum ProcessingResult
    {
        /// <summary>
        /// The event was not processed because the state machine is already processing another event.
        /// This prevents reentrancy issues where events are triggered during callback handling.
        /// </summary>
        NotProcessed = 0,

        /// <summary>
        /// The event was processed, but the state machine remained in the same state.
        /// </summary>
        Remained,

        /// <summary>
        /// The event was processed and the state machine transitioned to a new state.
        /// </summary>
        Transitioned
    }
}
