namespace Machinery;

using System.Diagnostics.CodeAnalysis;

internal sealed partial class Door
{
    internal sealed class Locked : State
    {
        private Locked() { }

        internal static Locked Instance { get; } = new();

        public override bool TryCreateNewState(Door context, Event ev, [NotNullWhen(true)] out State? newState) =>
            ev switch
            {
                Event.Unlock => StateHelpers.Transit(Closed.Instance, out newState),
                _ => StateHelpers.Ignore(out newState)
            };

        public override void OnExiting(Door context, Event ev, State newState) =>
            context.OnExitingLocked(ev, this, newState);

        public override void OnRemain(Door context, Event ev) => context.OnRemainLocked(ev, this);

        public override void OnEntered(Door context, Event ev, State oldState) =>
            context.OnEnteredLocked(ev, this, oldState);
    }
}
