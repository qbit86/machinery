namespace Machinery
{
    internal sealed partial class Door
    {
        internal sealed class Opened : State
        {
            private Opened() { }

            internal static Opened Instance { get; } = new();

            public override bool TryCreateNewState(Door context, Event ev, out State newState) =>
                ev switch
                {
                    Event.Interact => StateHelpers.Transit(Closed.Instance, out newState),
                    _ => StateHelpers.Ignore(out newState)
                };

            public override void OnExiting(Door context, Event ev, State newState) =>
                context.OnExitingOpened(ev, this, newState);

            public override void OnRemain(Door context, Event ev) => context.OnRemainOpened(ev, this);

            public override void OnEntered(Door context, Event ev, State oldState) =>
                context.OnEnteredOpened(ev, this, oldState);
        }
    }
}
