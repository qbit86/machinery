namespace Machinery;

using System;

internal static class ContextDemo
{
    private static void Main()
    {
        Door door = new(Console.Out);
        StateMachine<Door, Event, State> stateMachine = new(door, Door.CreateInitialState());

        stateMachine.TryProcessEvent(Event.Unlock);
        stateMachine.TryProcessEvent(Event.Interact);
        stateMachine.TryProcessEvent(Event.Lock);
        stateMachine.TryProcessEvent(Event.Interact);
    }
}
