namespace Machinery
{
    public interface IPolicy<in TContext, in TEvent, TState>
    {
        bool TryCreateNewState(TContext context, TEvent ev, TState currentState, out TState newState);
        void OnExiting(TContext context, TEvent ev, TState currentState, TState newState);
        void OnRemain(TContext context, TEvent ev, TState currentState);
        void OnEntered(TContext context, TEvent ev, TState currentState, TState oldState);
    }
}
