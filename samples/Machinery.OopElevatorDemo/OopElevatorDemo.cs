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
            return $"{Kind}({Floor})";
        }
    }

    internal abstract class StateBase : IState<StateBase, Event, TextWriter>, IDisposable
    {
        internal StateBase(int floor)
        {
            Floor = floor;
        }

        internal int Floor { get; }

        public void Dispose() { }

        public abstract bool TryCreateNewState(TextWriter context, Event ev, out StateBase newState);

        public void OnExiting(TextWriter context, Event ev, StateBase newState)
        {
            context.Write($"[{GetType().Name}.{nameof(OnEntered)}] ");
            context.WriteLine($"{nameof(Floor)}: {Floor}, {nameof(ev)}: {ev}, {nameof(newState)}: {newState}");
        }

        public void OnEntered(TextWriter context, Event ev, StateBase oldState)
        {
            context.Write($"[{GetType().Name}.{nameof(OnEntered)}] ");
            context.WriteLine($"{nameof(Floor)}: {Floor}, {nameof(ev)}: {ev}, {nameof(oldState)}: {oldState}");
        }

        public sealed override string ToString()
        {
            return $"{GetType().Name}({Floor})";
        }

        protected static bool Transit(StateBase newState, out StateBase result)
        {
            result = newState;
            return true;
        }

        protected static bool Ignore(out StateBase result)
        {
            result = default;
            return false;
        }
    }

    internal sealed class IdleState : StateBase
    {
        internal IdleState(int floor) : base(floor) { }

        public override bool TryCreateNewState(TextWriter context, Event ev, out StateBase newState)
        {
            switch (ev.Kind)
            {
                case EventKind.Call:
                case EventKind.Move:
                    if (Floor == ev.Floor)
                        return Ignore(out newState);
                    return Transit(new MovingState(ev.Floor), out newState);
                default:
                    return Ignore(out newState);
            }
        }
    }

    internal sealed class MovingState : StateBase
    {
        internal MovingState(int floor) : base(floor) { }

        public override bool TryCreateNewState(TextWriter context, Event ev, out StateBase newState)
        {
            switch (ev.Kind)
            {
                case EventKind.Stop:
                    return Transit(new IdleState(Floor), out newState);
                default:
                    return Ignore(out newState);
            }
        }
    }

    internal static class OopElevatorDemo
    {
        private static TextWriter Out => Console.Out;

        private static void Main()
        {
            var elevatorEventSink = new ContextBoundEventSink<StateBase, Event, TextWriter>(Out);
            StateMachine<StateBase, Event, ContextBoundEventSink<StateBase, Event, TextWriter>> elevator =
                StateMachine<Event>.Create((StateBase)new IdleState(0), elevatorEventSink);
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
            this StateMachine<StateBase, Event, ContextBoundEventSink<StateBase, Event, TextWriter>> elevator)
        {
            Out.WriteLine($"[{nameof(PrintCurrentState)}] {nameof(elevator.CurrentState)}: {elevator.CurrentState}");
        }

        private static void PrintProcessEvent(
            this StateMachine<StateBase, Event, ContextBoundEventSink<StateBase, Event, TextWriter>> elevator, Event ev)
        {
            Out.WriteLine($"[{nameof(PrintProcessEvent)}] {nameof(ev)}: {ev}");
            elevator.ProcessEvent(ev);
        }
    }
}
