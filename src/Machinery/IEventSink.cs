namespace Machinery
{
    public interface IEventSink<TState, in TEvent>
    {
        bool TryCreateNewState(TState currentState, TEvent ev, out TState newState);
        void OnExiting(TState currentState, TEvent ev, TState newState);
        void OnEntered(TState currentState, TEvent ev, TState oldState);
        void DisposeState(TState stateToDispose);
    }
}