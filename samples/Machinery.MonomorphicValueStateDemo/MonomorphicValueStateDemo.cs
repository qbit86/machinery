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

    internal enum StateKind
    {
        None = 0,
        IdleDown,
        IdleUp,
        MovingDown,
        MovingUp
    }

    internal readonly struct State : IState<TextWriter, Event, State>
    {
        public State(StateKind kind) => Kind = kind;

        internal StateKind Kind { get; }

        public override string ToString() => Kind.ToString();

        public bool TryCreateNewState(TextWriter context, Event ev, out State newState)
        {
            switch (Kind)
            {
                case StateKind.IdleDown:
                    switch (ev)
                    {
                        case Event.CallUp:
                        case Event.Move:
                            return Transit(new(StateKind.MovingUp), out newState);
                        default:
                            return Ignore(out newState);
                    }
                case StateKind.IdleUp:
                    switch (ev)
                    {
                        case Event.CallDown:
                        case Event.Move:
                            return Transit(new(StateKind.MovingDown), out newState);
                        default:
                            return Ignore(out newState);
                    }
                case StateKind.MovingDown:
                    switch (ev)
                    {
                        case Event.Stop:
                            return Transit(new(StateKind.IdleDown), out newState);
                        default:
                            return Ignore(out newState);
                    }
                case StateKind.MovingUp:
                    switch (ev)
                    {
                        case Event.Stop:
                            return Transit(new(StateKind.IdleUp), out newState);
                        default:
                            return Ignore(out newState);
                    }
                default:
                    return Ignore(out newState);
            }
        }

        public void OnExiting(TextWriter context, Event ev, State newState)
        {
            const string tag = nameof(State) + "." + nameof(OnExiting);
            context.WriteLine(
                $"[{tag}] {nameof(ev)}: {ev}, this: {ToString()}, {nameof(newState)}: {newState.ToString()}");
        }

        public void OnRemain(TextWriter context, Event ev)
        {
            const string tag = nameof(State) + "." + nameof(OnRemain);
            context.WriteLine(
                $"[{tag}] {nameof(ev)}: {ev}, this: {ToString()}");
        }

        public void OnEntered(TextWriter context, Event ev, State oldState)
        {
            const string tag = nameof(State) + "." + nameof(OnEntered);
            context.WriteLine(
                $"[{tag}] {nameof(ev)}: {ev}, this: {ToString()}, {nameof(oldState)}: {oldState.ToString()}");
        }

        private static bool Transit(State newState, out State result)
        {
            result = newState;
            return true;
        }

        private static bool Ignore(out State result)
        {
            result = default;
            return false;
        }
    }

    internal static class MonomorphicValueStateDemo
    {
        private static TextWriter Out => Console.Out;

        private static void Main()
        {
            StateMachine<TextWriter, Event, State> elevator = new(Out, new(StateKind.IdleDown));

            elevator.TryProcessEvent(Event.CallDown);

            Out.WriteLine();
            elevator.TryProcessEvent(Event.CallUp);

            Out.WriteLine();
            elevator.TryProcessEvent(Event.Stop);
        }
    }
}
