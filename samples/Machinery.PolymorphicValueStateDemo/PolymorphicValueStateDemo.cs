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
        internal abstract bool TryCreateNewState(TextWriter context, Event ev, in State state, out State newState);
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

        internal static IdleStateMethodTable Default { get; } = new();

        internal override bool TryCreateNewState(TextWriter context, Event ev, in State state, out State newState)
        {
            switch (ev.Kind)
            {
                case EventKind.Call:
                case EventKind.Move:
                    if (state.Floor == ev.Floor)
                        return Ignore(out newState);
                    return Transit(new(ev.Floor, MovingStateMethodTable.Default), out newState);
                default:
                    return Ignore(out newState);
            }
        }

        internal override string ToString(int floor) => $"Idle({floor})";
    }

    internal sealed class MovingStateMethodTable : StateMethodTable
    {
        private MovingStateMethodTable() { }

        internal static MovingStateMethodTable Default { get; } = new();

        internal override bool TryCreateNewState(TextWriter context, Event ev, in State state, out State newState)
        {
            switch (ev.Kind)
            {
                case EventKind.Stop:
                    return Transit(new(state.Floor, IdleStateMethodTable.Default), out newState);
                default:
                    return Ignore(out newState);
            }
        }

        internal override string ToString(int floor) => $"Moving({floor})";
    }

    internal readonly struct State : IState<TextWriter, Event, State>, IDisposable
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

        public bool TryCreateNewState(TextWriter context, Event ev, out State newState) =>
            _stateMethodTable.TryCreateNewState(context, ev, this, out newState);

        public void OnExiting(TextWriter context, Event ev, State newState)
        {
            const string tag = nameof(State) + "." + nameof(OnExiting);
            context.WriteLine($"[{tag}] {nameof(ev)}: {ev}, this: {this}, {nameof(newState)}: {newState}");
        }

        public void OnRemain(TextWriter context, Event ev)
        {
            const string tag = nameof(State) + "." + nameof(OnRemain);
            context.WriteLine($"[{tag}] {nameof(ev)}: {ev}, this: {this}");
        }

        public void OnEntered(TextWriter context, Event ev, State oldState)
        {
            const string tag = nameof(State) + "." + nameof(OnEntered);
            context.WriteLine($"[{tag}] {nameof(ev)}: {ev}, this: {this}, {nameof(oldState)}: {oldState}");
        }

        public override string ToString() => _stringRepresentation;
    }

    internal static class PolymorphicValueStateDemo
    {
        private static TextWriter Out => Console.Out;

        private static void Main()
        {
            StateMachine<TextWriter, Event, State> elevator = new(Out, new(0, IdleStateMethodTable.Default));

            elevator.TryProcessEvent(new(EventKind.Call, -1));

            Out.WriteLine();
            elevator.TryProcessEvent(new(EventKind.Call, 2));

            Out.WriteLine();
            elevator.TryProcessEvent(new(EventKind.Stop, default));
        }
    }
}
