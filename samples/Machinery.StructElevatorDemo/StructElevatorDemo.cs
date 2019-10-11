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

    internal abstract class StateMethodTable
    {
        internal abstract bool TryCreateNewState(in State state, TextWriter context, Event ev, out State newState);
        internal abstract string ToString(int floor);

        protected static bool Transit(State newState, out State result)
        {
            result = newState;
            return true;
        }

        protected static bool Ignore(out State result)
        {
            result = default;
            return false;
        }
    }

    internal sealed class IdleStateMethodTable : StateMethodTable
    {
        private IdleStateMethodTable() { }

        internal static IdleStateMethodTable Default { get; } = new IdleStateMethodTable();

        internal override bool TryCreateNewState(in State state, TextWriter context, Event ev, out State newState)
        {
            switch (ev.Kind)
            {
                case EventKind.Call:
                case EventKind.Move:
                    if (state.Floor == ev.Floor)
                        return Ignore(out newState);
                    return Transit(new State(ev.Floor, MovingStateMethodTable.Default), out newState);
                default:
                    return Ignore(out newState);
            }
        }

        internal override string ToString(int floor)
        {
            return $"Idle({floor})";
        }
    }

    internal sealed class MovingStateMethodTable : StateMethodTable
    {
        private MovingStateMethodTable() { }

        internal static MovingStateMethodTable Default { get; } = new MovingStateMethodTable();

        internal override bool TryCreateNewState(in State state, TextWriter context, Event ev, out State newState)
        {
            switch (ev.Kind)
            {
                case EventKind.Stop:
                    return Transit(new State(state.Floor, IdleStateMethodTable.Default), out newState);
                default:
                    return Ignore(out newState);
            }
        }

        internal override string ToString(int floor)
        {
            return $"Moving({floor})";
        }
    }

    internal readonly struct State : IState<State, Event, TextWriter>, IDisposable
    {
        private readonly StateMethodTable _stateMethodTable;
        private readonly string _stringRepresentation;

        internal State(int floor, StateMethodTable stateMethodTable)
        {
            _stateMethodTable = stateMethodTable ?? throw new ArgumentNullException(nameof(stateMethodTable));
            _stringRepresentation = stateMethodTable.ToString(floor);
            Floor = floor;
        }

        internal int Floor { get; }

        public void Dispose() { }

        public bool TryCreateNewState(TextWriter context, Event ev, out State newState)
        {
            return _stateMethodTable.TryCreateNewState(this, context, ev, out newState);
        }

        public void OnExiting(TextWriter context, Event ev, State newState)
        {
            context.Write($"[{GetType().Name}.{nameof(OnExiting)}] ");
            context.WriteLine($"this: {this}, {nameof(ev)}: {ev}, {nameof(newState)}: {newState}");
        }

        public void OnEntered(TextWriter context, Event ev, State oldState)
        {
            context.Write($"[{GetType().Name}.{nameof(OnEntered)}] ");
            context.WriteLine($"this: {this}, {nameof(ev)}: {ev}, {nameof(oldState)}: {oldState}");
        }

        public override string ToString()
        {
            return _stringRepresentation;
        }
    }

    internal static class StructElevatorDemo
    {
        private static TextWriter Out => Console.Out;

        private static void Main()
        {
            var elevatorEventSink = new ContextBoundPolicy<State, Event, TextWriter>(Out);
            StateMachine<TextWriter, State, Event, ContextBoundPolicy<State, Event, TextWriter>> elevator =
                StateMachine<Event>.Create(Out, new State(0, IdleStateMethodTable.Default), elevatorEventSink);
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
            this StateMachine<TextWriter, State, Event, ContextBoundPolicy<State, Event, TextWriter>> elevator)
        {
            Out.WriteLine($"[{nameof(PrintCurrentState)}] {nameof(elevator.CurrentState)}: {elevator.CurrentState}");
        }

        private static void PrintProcessEvent(
            this StateMachine<TextWriter, State, Event, ContextBoundPolicy<State, Event, TextWriter>> elevator,
            Event ev)
        {
            Out.WriteLine($"[{nameof(PrintProcessEvent)}] {nameof(ev)}: {ev}");
            elevator.TryProcessEvent(ev);
        }
    }
}
