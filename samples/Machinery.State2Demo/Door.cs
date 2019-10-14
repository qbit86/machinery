namespace Machinery
{
    internal sealed class Door
    {
        private Door() { }

        internal static Door Instance { get; } = new Door();

        internal void OnExitingOpened(Event ev, Opened currentState, IState<Door, Event> newState) { }

        internal void OnRemainOpened(Event ev, Opened currentState) { }

        internal void OnEnteredOpened(Event ev, Opened currentState, IState<Door, Event> oldState) { }

        internal void OnExitingClosed(Event ev, Closed currentState, IState<Door, Event> newState) { }

        internal void OnRemainClosed(Event ev, Closed currentState) { }

        internal void OnEnteredClosed(Event ev, Closed currentState, IState<Door, Event> oldState) { }

        internal void OnExitingLocked(Event ev, Locked currentState, IState<Door, Event> newState) { }

        internal void OnRemainLocked(Event ev, Locked currentState) { }

        internal void OnEnteredLocked(Event ev, Locked currentState, IState<Door, Event> oldState) { }
    }
}
