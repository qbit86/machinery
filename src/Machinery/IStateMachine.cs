namespace Machinery
{
    public interface IStateMachine<out TState, in TEvent> : IEventSource<TEvent>, IStateHolder<TState> { }
}
