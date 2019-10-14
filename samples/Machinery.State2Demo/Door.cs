namespace Machinery
{
    internal sealed partial class Door
    {
        private Door() { }

        internal static Door Instance { get; } = new Door();

        private void OnExitingOpened(Event ev, Opened currentState, IState<Door, Event> newState) { }

        private void OnRemainOpened(Event ev, Opened currentState) { }

        private void OnEnteredOpened(Event ev, Opened currentState, IState<Door, Event> oldState) { }

        private void OnExitingClosed(Event ev, Closed currentState, IState<Door, Event> newState) { }

        private void OnRemainClosed(Event ev, Closed currentState) { }

        private void OnEnteredClosed(Event ev, Closed currentState, IState<Door, Event> oldState) { }

        private void OnExitingLocked(Event ev, Locked currentState, IState<Door, Event> newState) { }

        private void OnRemainLocked(Event ev, Locked currentState) { }

        private void OnEnteredLocked(Event ev, Locked currentState, IState<Door, Event> oldState) { }
    }
}
