namespace Machinery
{
    using System;

    internal enum Event
    {
        None = 0,

        // The same event used for both opening and closing.
        Interact,
        Lock,
        Unlock
    }

    internal static class State2Demo
    {
        private static void Main()
        {
            var door = new Door(Console.Out);
            var stateMachine = new StateMachine<Door, Event>(door, door.CreateInitialState());

            stateMachine.TryProcessEvent(Event.Unlock);
            stateMachine.TryProcessEvent(Event.Interact);
            stateMachine.TryProcessEvent(Event.Lock);
            stateMachine.TryProcessEvent(Event.Interact);
        }
    }
}
