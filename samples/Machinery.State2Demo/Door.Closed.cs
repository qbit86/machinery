namespace Machinery
{
    internal sealed partial class Door
    {
        internal sealed class Closed : State
        {
            private Closed() { }

            internal static Closed Instance { get; } = new();

            public override bool TryCreateNewState(Door context, Event ev, out State newState) =>
                ev switch
                {
                    Event.Interact => StateHelpers.Transit(Opened.Instance, out newState),
                    Event.Lock => StateHelpers.Transit(Locked.Instance, out newState),
                    _ => StateHelpers.Ignore(out newState)
                };

            public override void OnExiting(Door context, Event ev, State newState) =>
                context.OnExitingClosed(ev, this, newState);

            public override void OnRemain(Door context, Event ev) => context.OnRemainClosed(ev, this);

            public override void OnEntered(Door context, Event ev, State oldState) =>
                context.OnEnteredClosed(ev, this, oldState);
        }
    }
}
