namespace Machinery
{
    using System;

    internal sealed class Locked : IState<Door, Event>
    {
        public bool TryCreateNewState(Door context, Event ev, out IState<Door, Event> newState)
        {
            throw new NotImplementedException();
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
