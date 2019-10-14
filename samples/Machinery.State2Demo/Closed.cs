namespace Machinery
{
    using System;

    internal sealed class Closed : IState<Door, Event>
    {
        private Closed() { }

        internal static Closed Instance { get; } = new Closed();

        public bool TryCreateNewState(Door context, Event ev, out IState<Door, Event> newState)
        {
            return ev switch
            {
                Event.Interact => StateHelpers.Transit(Opened.Instance, out newState),
                Event.Lock => StateHelpers.Transit(Locked.Instance, out newState),
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
