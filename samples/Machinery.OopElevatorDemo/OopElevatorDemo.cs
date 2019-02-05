﻿namespace Machinery
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }

    internal static class OopElevatorDemo
    {
        private static TextWriter Out => Console.Out;

        private static void Main()
        {
            Console.WriteLine("Hello World!");
        }
    }
}
