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

    internal abstract class StateBase : IState<StateBase, Event, TextWriter>
    {
        public abstract bool TryCreateNewState(TextWriter context, Event ev, out StateBase newState);

        public void OnExiting(TextWriter context, Event ev, StateBase newState)
        {
            context.WriteLine(
                $"[{GetType().Name}.{nameof(OnExiting)}] {nameof(ev)}: {ev}, {nameof(newState)}: {newState}");
        }

        public void OnEntered(TextWriter context, Event ev, StateBase oldState)
        {
            context.WriteLine(
                $"[{GetType().Name}.{nameof(OnEntered)}] {nameof(ev)}: {ev}, {nameof(oldState)}: {oldState}");
        }

        public void Dispose()
        {
        }

        public sealed override string ToString()
        {
            return GetType().Name;
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

    internal sealed class IdleDownState : StateBase
    {
        private IdleDownState()
        {
        }

        internal static IdleDownState Default { get; } = new IdleDownState();

        public override bool TryCreateNewState(TextWriter context, Event ev, out StateBase newState)
        {
            switch (ev)
            {
                case Event.CallUp:
                case Event.Move:
                    return Transit(MovingUpState.Default, out newState);
                default:
                    return Ignore(out newState);
            }
        }
    }

    internal sealed class IdleUpState : StateBase
    {
        private IdleUpState()
        {
        }

        internal static IdleUpState Default { get; } = new IdleUpState();

        public override bool TryCreateNewState(TextWriter context, Event ev, out StateBase newState)
        {
            switch (ev)
            {
                case Event.CallDown:
                case Event.Move:
                    return Transit(MovingDownState.Default, out newState);
                default:
                    return Ignore(out newState);
            }
        }
    }

    internal sealed class MovingDownState : StateBase
    {
        private MovingDownState()
        {
        }

        internal static MovingDownState Default { get; } = new MovingDownState();

        public override bool TryCreateNewState(TextWriter context, Event ev, out StateBase newState)
        {
            switch (ev)
            {
                case Event.Stop:
                    return Transit(IdleDownState.Default, out newState);
                default:
                    return Ignore(out newState);
            }
        }
    }

    internal sealed class MovingUpState : StateBase
    {
        private MovingUpState()
        {
        }

        internal static MovingUpState Default { get; } = new MovingUpState();

        public override bool TryCreateNewState(TextWriter context, Event ev, out StateBase newState)
        {
            switch (ev)
            {
                case Event.Stop:
                    return Transit(IdleUpState.Default, out newState);
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
            var elevatorPolicy = new ContextStatePolicy<StateBase, Event, TextWriter>(Out);
            StateMachine<StateBase, Event, ContextStatePolicy<StateBase, Event, TextWriter>> elevator =
                StateMachine<Event>.Create((StateBase)IdleDownState.Default, elevatorPolicy);
            elevator.PrintCurrentState();
            Out.WriteLine();

            elevator.ProcessEvent(Event.CallDown);
            elevator.PrintCurrentState();
            Out.WriteLine();

            elevator.ProcessEvent(Event.CallUp);
            elevator.PrintCurrentState();
            Out.WriteLine();

            elevator.ProcessEvent(Event.Stop);
            elevator.PrintCurrentState();
            Out.WriteLine();
        }

        private static void PrintCurrentState(
            this StateMachine<StateBase, Event, ContextStatePolicy<StateBase, Event, TextWriter>> elevator)
        {
            Out.WriteLine($"[{nameof(PrintCurrentState)}] {nameof(elevator.CurrentState)}: {elevator.CurrentState}");
        }

        private static void ProcessEvent(
            this StateMachine<StateBase, Event, ContextStatePolicy<StateBase, Event, TextWriter>> elevator, Event ev)
        {
            Out.WriteLine($"[{nameof(ProcessEvent)}] {nameof(ev)}: {ev}");
            elevator.Process(ev);
        }
    }
}
