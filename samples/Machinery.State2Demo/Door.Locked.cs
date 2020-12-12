namespace Machinery
{
    internal sealed partial class Door
    {
        internal sealed class Locked : IState<Door, Event>
        {
            private Locked() { }

            internal static Locked Instance { get; } = new Locked();

            public bool TryCreateNewState(Door context, Event ev, out IState<Door, Event> newState) =>
                ev switch
                {
                    Event.Unlock => StateHelpers.Transit(Closed.Instance, out newState),
                    _ => StateHelpers.Ignore(out newState)
                };

            public void OnExiting(Door context, Event ev, IState<Door, Event> newState) =>
                context.OnExitingLocked(ev, this, newState);

            public void OnRemain(Door context, Event ev) => context.OnRemainLocked(ev, this);

            public void OnEntered(Door context, Event ev, IState<Door, Event> oldState) =>
                context.OnEnteredLocked(ev, this, oldState);
        }
    }
}
