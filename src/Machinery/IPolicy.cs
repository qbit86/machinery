namespace Machinery
{
    public interface IPolicy<TState, in TEvent>
    {
        bool TryCreateNewState(TState currentState, TEvent ev, out TState newState);
        void OnExiting(TState currentState, TEvent ev, TState newState);
        void OnEntered(TState currentState, TEvent ev, TState oldState);
        void DisposeState(TState stateToDispose, TEvent ev);
    }
}
