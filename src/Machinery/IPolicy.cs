namespace Machinery
{
    public interface IPolicy<in TContext, in TEvent, TState>
    {
        bool TryCreateNewState(TContext context, TState currentState, TEvent ev, out TState newState);
        void OnExiting(TContext context, TState currentState, TEvent ev, TState newState);
        void OnEntered(TContext context, TState currentState, TEvent ev, TState oldState);
        void DisposeState(TContext context, TState stateToDispose, TEvent ev);
    }
}
