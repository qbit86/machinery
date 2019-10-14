namespace Machinery
{
    using System;
    using System.IO;

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
        private static TextWriter Out => Console.Out;

        private static void Main()
        {
            var door = new Door(Out);
            var stateMachine = new StateMachine<Door, Event>(door, door.CreateInitialState());

            stateMachine.TryProcessEvent(Event.Lock);
            stateMachine.TryProcessEvent(Event.Interact);
            stateMachine.TryProcessEvent(Event.Unlock);
            stateMachine.TryProcessEvent(Event.Lock);
        }
    }
}
