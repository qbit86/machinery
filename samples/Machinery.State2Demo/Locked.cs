namespace Machinery
{
    using System;

    internal sealed class Locked : IState<Door, Event>
    {
        private Locked() { }

        internal static Locked Instance { get; } = new Locked();

        public bool TryCreateNewState(Door context, Event ev, out IState<Door, Event> newState)
        {
            return ev switch
            {
                Event.Unlock => StateHelpers.Transit(Closed.Instance, out newState),
                _ => StateHelpers.Ignore(out newState)
            };
        }

        public void OnExiting(Door context, Event ev, IState<Door, Event> newState)
        {
            throw new NotImplementedException();
        }

        public void OnRemain(Door context, Event ev, IState<Door, Event> currentState)
        {
            throw new NotImplementedException();
        }

        public void OnEntered(Door context, Event ev, IState<Door, Event> oldState)
        {
            throw new NotImplementedException();
        }
    }
}
