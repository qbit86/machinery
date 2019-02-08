namespace Machinery
{
    public interface IContextAwareStateMachine<out TState, in TEvent, in TContext> :
        IContextAwareEventSource<TEvent, TContext>, IStateHolder<TState> { }
}
