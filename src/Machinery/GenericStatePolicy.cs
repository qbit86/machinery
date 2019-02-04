namespace Machinery
{
    using System;
    using System.Runtime.CompilerServices;

    public readonly struct GenericStatePolicy<TState, TEvent, TContext> : IStatePolicy<TState, TEvent, TContext>,
        IEquatable<GenericStatePolicy<TState, TEvent, TContext>>
        where TState : IState<TState, TEvent, TContext>
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

        public void DisposeState(TContext context, TState currentState, TEvent ev, TState stateToDispose)
        {
            stateToDispose.Dispose();
        }

        public bool Equals(GenericStatePolicy<TState, TEvent, TContext> other)
        {
            return true;
        }

        public override bool Equals(object obj)
        {
            return obj is GenericStatePolicy<TState, TEvent, TContext>;
        }

        public override int GetHashCode()
        {
            return typeof(GenericStatePolicy<TState, TEvent, TContext>).GetHashCode();
        }

#pragma warning disable CA1801
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(GenericStatePolicy<TState, TEvent, TContext> left,
            GenericStatePolicy<TState, TEvent, TContext> right)
        {
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(GenericStatePolicy<TState, TEvent, TContext> left,
            GenericStatePolicy<TState, TEvent, TContext> right)
        {
            return false;
        }
#pragma warning restore CA1801
    }
}
