namespace Machinery
{
    public interface IContextAwareEventSink<TState, in TEvent, in TContext>
    {
        bool TryCreateNewState(TContext context, TState currentState, TEvent ev, out TState newState);
        void OnExiting(TContext context, TState currentState, TEvent ev, TState newState);
        void OnEntered(TContext context, TState currentState, TEvent ev, TState oldState);
        void DisposeState(TState stateToDispose);
    }
}
