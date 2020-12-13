namespace Machinery
{
    internal sealed partial class Door
    {
        internal sealed class Closed : IState<Door, Event>
        {
            private Closed() { }

            internal static Closed Instance { get; } = new();

            public bool TryCreateNewState(Door context, Event ev, out IState<Door, Event> newState) =>
                ev switch
                {
                    Event.Interact => StateHelpers.Transit(Opened.Instance, out newState),
                    Event.Lock => StateHelpers.Transit(Locked.Instance, out newState),
                    _ => StateHelpers.Ignore(out newState)
                };

            public void OnExiting(Door context, Event ev, IState<Door, Event> newState) =>
                context.OnExitingClosed(ev, this, newState);

            public void OnRemain(Door context, Event ev) => context.OnRemainClosed(ev, this);

            public void OnEntered(Door context, Event ev, IState<Door, Event> oldState) =>
                context.OnEnteredClosed(ev, this, oldState);
        }
    }
}
