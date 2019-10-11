namespace Machinery
{
    public interface IState<in TContext, in TEvent, TState>
    {
        bool TryCreateNewState(TContext context, TEvent ev, out TState newState);
        void OnExiting(TContext context, TEvent ev, TState newState);
        void OnRemain(TContext context, TEvent ev, TState currentState);
        void OnEntered(TContext context, TEvent ev, TState oldState);
    }
}
