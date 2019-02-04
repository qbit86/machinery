namespace Machinery
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;

    public readonly struct GenericStatePolicy<TContext, TState, TEvent> : IStatePolicy<TContext, TState, TEvent>,
        IEquatable<GenericStatePolicy<TContext, TState, TEvent>>
        where TState : IState<TContext, TState, TEvent>
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

        public bool Equals(GenericStatePolicy<TContext, TState, TEvent> other)
        {
            return true;
        }

        public override bool Equals(object obj)
        {
            return obj is GenericStatePolicy<TContext, TState, TEvent>;
        }

        public override int GetHashCode()
        {
            return typeof(GenericStatePolicy<TContext, TState, TEvent>).GetHashCode();
        }

#pragma warning disable CA1801
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(GenericStatePolicy<TContext, TState, TEvent> left,
            GenericStatePolicy<TContext, TState, TEvent> right)
        {
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(GenericStatePolicy<TContext, TState, TEvent> left,
            GenericStatePolicy<TContext, TState, TEvent> right)
        {
            return false;
        }
#pragma warning restore CA1801
    }
}
