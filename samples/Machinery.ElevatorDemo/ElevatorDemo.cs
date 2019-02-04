namespace Machinery
{
    using System;
    using System.IO;

    internal enum State
    {
        None = 0,
        IdleDown,
        IdleUp,
        MovingDown,
        MovingUp
    }

    internal enum Event
    {
        None = 0,
        CallDown,
        CallUp,
        Move,
        Stop
    }

    internal readonly struct ElevatorPolicy : IStatePolicy<State, Event>
    {
        private TextWriter Out { get; }

        public ElevatorPolicy(TextWriter @out)
        {
            Out = @out;
        }

        public bool TryCreateNewState(State currentState, Event ev, out State newState)
        {
            switch (currentState)
            {
                case State.IdleDown:
                    throw new NotImplementedException();
                case State.IdleUp:
                    throw new NotImplementedException();
                case State.MovingDown:
                    throw new NotImplementedException();
                case State.MovingUp:
                    throw new NotImplementedException();
                default:
                    newState = default;
                    return false;
            }
        }

        public void OnExiting(State currentState, Event ev, State newState)
        {
            Out.WriteLine($"{nameof(OnExiting)}({ev}): {currentState} -> {newState}");
        }

        public void OnEntered(State currentState, Event ev, State oldState)
        {
            Out.WriteLine($"{nameof(OnEntered)}({ev}): {oldState} -> {currentState}");
        }

        public void DisposeState(State currentState, Event ev, State stateToDispose)
        {
        }
    }

    internal static class ElevatorDemo
    {
        private static TextWriter Out => Console.Out;

        private static void Main()
        {
            StateMachine<State, Event, ElevatorPolicy> elevator =
                StateMachine<Event>.Create(State.IdleDown, new ElevatorPolicy(Out));
            Out.WriteLine($"{nameof(elevator.CurrentState)}: {elevator.CurrentState}");

            elevator.Process(Event.CallDown);
            Out.WriteLine($"{nameof(elevator.CurrentState)}: {elevator.CurrentState}");
        }
    }
}
