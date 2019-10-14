namespace Machinery
{
    using System;

    internal sealed class Opened : IState<Door, Event>
    {
        private Opened() { }

        internal static Opened Instance { get; } = new Opened();

        public bool TryCreateNewState(Door context, Event ev, out IState<Door, Event> newState)
        {
            return ev switch
            {
                Event.Interact => StateHelpers.Transit(Closed.Instance, out newState),
                _ => StateHelpers.Ignore(out newState)
            };
        }

        public void OnExiting(Door context, Event ev, IState<Door, Event> newState)
        {
            context.OnExitingOpened(ev, this, newState);
        }

        public void OnRemain(Door context, Event ev, IState<Door, Event> currentState)
        {
            context.OnRemainOpened(ev, this);
        }

        public void OnEntered(Door context, Event ev, IState<Door, Event> oldState)
        {
            context.OnEnteredOpened(ev, this, oldState);
        }
    }
}
