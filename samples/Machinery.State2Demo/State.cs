namespace Machinery
{
    internal abstract class State : IState<Door, Event, State>
    {
        public abstract bool TryCreateNewState(Door context, Event ev, out State newState);

        public abstract void OnExiting(Door context, Event ev, State newState);

        public abstract void OnRemain(Door context, Event ev);

        public abstract void OnEntered(Door context, Event ev, State oldState);
    }
}
