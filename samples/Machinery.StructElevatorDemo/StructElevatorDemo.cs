namespace Machinery
{
    using System;
    using System.IO;

    internal enum EventKind
    {
        None = 0,
        Call,
        Move,
        Stop
    }

    internal readonly struct Event
    {
        public Event(EventKind kind, int floor)
        {
            Kind = kind;
            Floor = floor;
        }

        public EventKind Kind { get; }
        public int Floor { get; }

        public override string ToString()
        {
            if (Kind == EventKind.Stop)
                return nameof(EventKind.Stop);

            return $"{Kind}({Floor})";
        }
    }

    internal readonly struct State : IState<State, Event, TextWriter>, IDisposable
    {
        internal State(int floor)
        {
            Floor = floor;
        }

        internal int Floor { get; }

        public bool TryCreateNewState(TextWriter context, Event ev, out State newState)
        {
            throw new NotImplementedException();
        }

        public void OnExiting(TextWriter context, Event ev, State newState)
        {
            throw new NotImplementedException();
        }

        public void OnEntered(TextWriter context, Event ev, State oldState)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }
    }

    internal static class StructElevatorDemo
    {
        private static TextWriter Out => Console.Out;

        private static void Main()
        {
            var elevatorEventSink = new ContextBoundEventSink<State, Event, TextWriter>(Out);
            StateMachine<State, Event, ContextBoundEventSink<State, Event, TextWriter>> elevator =
                StateMachine<Event>.Create(new State(0), elevatorEventSink);
            elevator.PrintCurrentState();
            Out.WriteLine();

            elevator.PrintProcessEvent(new Event(EventKind.Call, -1));
            elevator.PrintCurrentState();
            Out.WriteLine();

            elevator.PrintProcessEvent(new Event(EventKind.Call, 2));
            elevator.PrintCurrentState();
            Out.WriteLine();

            elevator.PrintProcessEvent(new Event(EventKind.Stop, default));
            elevator.PrintCurrentState();
            Out.WriteLine();
        }

        private static void PrintCurrentState(
            this StateMachine<State, Event, ContextBoundEventSink<State, Event, TextWriter>> elevator)
        {
            Out.WriteLine($"[{nameof(PrintCurrentState)}] {nameof(elevator.CurrentState)}: {elevator.CurrentState}");
        }

        private static void PrintProcessEvent(
            this StateMachine<State, Event, ContextBoundEventSink<State, Event, TextWriter>> elevator, Event ev)
        {
            Out.WriteLine($"[{nameof(PrintProcessEvent)}] {nameof(ev)}: {ev}");
            elevator.ProcessEvent(ev);
        }
    }
}
