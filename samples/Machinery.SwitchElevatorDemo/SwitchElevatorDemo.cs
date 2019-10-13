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
            const string tag = nameof(ElevatorPolicy) + "." + nameof(OnExiting);
            context.WriteLine(
                $"[{tag}] {nameof(currentState)}: {currentState}, {nameof(ev)}: {ev}, {nameof(newState)}: {newState}");
        }

        public void OnRemain(TextWriter context, Event ev, State currentState)
        {
            const string tag = nameof(ElevatorPolicy) + "." + nameof(OnRemain);
            context.WriteLine(
                $"[{tag}] {nameof(currentState)}: {currentState}, {nameof(ev)}: {ev}");
        }

        public void OnEntered(TextWriter context, Event ev, State currentState, State oldState)
        {
            const string tag = nameof(ElevatorPolicy) + "." + nameof(OnEntered);
            context.WriteLine(
                $"[{tag}] {nameof(currentState)}: {currentState}, {nameof(ev)}: {ev}, {nameof(oldState)}: {oldState}");
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
            var elevatorPolicy = new ElevatorPolicy();
            StateMachine<TextWriter, Event, State, ElevatorPolicy> elevator =
                StateMachine<Event>.Create(Out, State.IdleDown, elevatorPolicy);

            elevator.TryProcessEvent(Event.CallDown);

            Out.WriteLine();
            elevator.TryProcessEvent(Event.CallUp);

            Out.WriteLine();
            elevator.TryProcessEvent(Event.Stop);
        }
    }
}
