namespace Machinery
{
    public interface IState<TContext, TEvent>
    {
        bool TryCreateNewState(TContext context, TEvent ev, out IState<TContext, TEvent> newState);
        void OnExiting(TContext context, TEvent ev, IState<TContext, TEvent> newState);
        void OnRemain(TContext context, TEvent ev, IState<TContext, TEvent> currentState);
        void OnEntered(TContext context, TEvent ev, IState<TContext, TEvent> oldState);
    }
}
