namespace Machinery
{
    using System;

#pragma warning disable CA1815, CA2231
    public readonly struct ContextAwareEventSink<TState, TEvent, TContext> :
        IContextAwareEventSink<TState, TEvent, TContext>,
        IEquatable<ContextAwareEventSink<TState, TEvent, TContext>>
        where TState : IState<TState, TEvent, TContext>
#pragma warning restore CA2231, CA1815
    {
        public bool TryCreateNewState(TContext context, TState currentState, TEvent ev, out TState newState)
        {
            return currentState.TryCreateNewState(context, ev, out newState);
        }

        public void OnExiting(TContext context, TState currentState, TEvent ev, TState newState)
        {
            currentState.OnExiting(context, ev, newState);
        }

        public void OnEntered(TContext context, TState currentState, TEvent ev, TState oldState)
        {
            currentState.OnEntered(context, ev, oldState);
        }

        public void DisposeState(TState stateToDispose)
        {
            stateToDispose.Dispose();
        }

        public bool Equals(ContextAwareEventSink<TState, TEvent, TContext> other)
        {
            return true;
        }

        public override bool Equals(object obj)
        {
            return obj is ContextAwareEventSink<TState, TEvent, TContext>;
        }

        public override int GetHashCode()
        {
            return typeof(ContextAwareEventSink<TState, TEvent, TContext>).GetHashCode();
        }
    }
}
