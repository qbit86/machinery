namespace Machinery
{
    using System;
    using System.IO;

    internal enum Event
    {
        None = 0,
        CallDown,
        CallUp,
        Move,
        Stop
    }

    internal enum State
    {
        None = 0,
        IdleDown,
        IdleUp,
        MovingDown,
        MovingUp
    }

    internal readonly struct ElevatorPolicy : IPolicy<TextWriter, Event, State>
    {
        private TextWriter Out { get; }

        public ElevatorPolicy(TextWriter @out)
        {
            Out = @out;
        }

        public bool TryCreateNewState(TextWriter context, Event ev, State currentState, out State newState)
        {
            switch (currentState)
            {
                case State.IdleDown:
                    switch (ev)
                    {
                        case Event.CallUp:
                        case Event.Move:
                            return Transit(State.MovingUp, out newState);
                        default:
                            return Ignore(out newState);
                    }
                case State.IdleUp:
                    switch (ev)
                    {
                        case Event.CallDown:
                        case Event.Move:
                            return Transit(State.MovingDown, out newState);
                        default:
                            return Ignore(out newState);
                    }
                case State.MovingDown:
                    switch (ev)
                    {
                        case Event.Stop:
                            return Transit(State.IdleDown, out newState);
                        default:
                            return Ignore(out newState);
                    }
                case State.MovingUp:
                    switch (ev)
                    {
                        case Event.Stop:
                            return Transit(State.IdleUp, out newState);
                        default:
                            return Ignore(out newState);
                    }
                default:
                    return Ignore(out newState);
            }
        }

        public void OnExiting(TextWriter context, Event ev, State currentState, State newState)
        {
            Out.Write($"[{GetType().Name}.{nameof(OnExiting)}] ");
            Out.WriteLine(
                $"{nameof(currentState)}: {currentState}, {nameof(ev)}: {ev}, {nameof(newState)}: {newState}");
        }

        public void OnEntered(TextWriter context, Event ev, State currentState, State oldState)
        {
            Out.Write($"[{GetType().Name}.{nameof(OnEntered)}] ");
            Out.WriteLine(
                $"{nameof(currentState)}: {currentState}, {nameof(ev)}: {ev}, {nameof(oldState)}: {oldState}");
        }

        public void DisposeState(TextWriter context, Event ev, State stateToDispose) { }

        private bool Transit(State newState, out State result)
        {
            result = newState;
            return true;
        }

        private bool Ignore(out State result)
        {
            result = default;
            return false;
        }
    }

    internal static class SwitchElevatorDemo
    {
        private static TextWriter Out => Console.Out;

        private static void Main()
        {
            var elevatorPolicy = new ElevatorPolicy(Out);
            StateMachine<TextWriter, Event, State, ElevatorPolicy> elevator =
                StateMachine<Event>.Create(Out, State.IdleDown, elevatorPolicy);
            elevator.PrintCurrentState();
            Out.WriteLine();

            elevator.PrintProcessEvent(Event.CallDown);
            elevator.PrintCurrentState();
            Out.WriteLine();

            elevator.PrintProcessEvent(Event.CallUp);
            elevator.PrintCurrentState();
            Out.WriteLine();

            elevator.PrintProcessEvent(Event.Stop);
            elevator.PrintCurrentState();
            Out.WriteLine();
        }

        private static void PrintCurrentState(this StateMachine<TextWriter, Event, State, ElevatorPolicy> elevator)
        {
            Out.WriteLine($"[{nameof(PrintCurrentState)}] {nameof(elevator.CurrentState)}: {elevator.CurrentState}");
        }

        private static void PrintProcessEvent(this StateMachine<TextWriter, Event, State, ElevatorPolicy> elevator,
            Event ev)
        {
            Out.WriteLine($"[{nameof(PrintProcessEvent)}] {nameof(ev)}: {ev}");
            elevator.TryProcessEvent(ev);
        }
    }
}
