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

        public abstract void OnExiting(TextWriter context, Event ev, StateBase newState);

        public abstract void OnEntered(TextWriter context, Event ev, StateBase oldState);

        public void Dispose()
        {
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

        public override void OnExiting(TextWriter context, Event ev, StateBase newState)
        {
            if (context == null || context == TextWriter.Null)
                return;

            throw new NotImplementedException();
        }

        public override void OnEntered(TextWriter context, Event ev, StateBase oldState)
        {
            if (context == null || context == TextWriter.Null)
                return;

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
