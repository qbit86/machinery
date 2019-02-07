namespace Machinery
{
    public interface IStateMachine<out TState, in TEvent> : IStateMachine<TEvent>
    {
        TState CurrentState { get; }
    }
}
