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
            throw new NotImplementedException();
        }
    }

    internal static class StructElevatorDemo
    {
        private static void Main()
        {
        }
    }
}
