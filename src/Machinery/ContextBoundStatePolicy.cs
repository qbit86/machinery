namespace Machinery
{
    using System;

#pragma warning disable CA1815, CA2231
    public readonly struct ContextBoundStatePolicy<TState, TEvent, TContext> : IStatePolicy<TState, TEvent>,
        IEquatable<ContextBoundStatePolicy<TState, TEvent, TContext>>
        where TState : IState<TState, TEvent, TContext>
#pragma warning restore CA2231, CA1815
    {
        private readonly TContext _context;

        public ContextBoundStatePolicy(TContext context)
        {
            _context = context;
        }

        public TContext Context => _context;

        public bool TryCreateNewState(TState currentState, TEvent ev, out TState newState)
        {
            return currentState.TryCreateNewState(_context, ev, out newState);
        }

        public void OnExiting(TState currentState, TEvent ev, TState newState)
        {
            currentState.OnExiting(_context, ev, newState);
        }

        public void OnEntered(TState currentState, TEvent ev, TState oldState)
        {
            currentState.OnEntered(_context, ev, oldState);
        }

        public void DisposeState(TState stateToDispose)
        {
            stateToDispose.Dispose();
        }

        public bool Equals(ContextBoundStatePolicy<TState, TEvent, TContext> other)
        {
            return true;
        }

        public override bool Equals(object obj)
        {
            return obj is ContextBoundStatePolicy<TState, TEvent, TContext>;
        }

        public override int GetHashCode()
        {
            return typeof(ContextBoundStatePolicy<TState, TEvent, TContext>).GetHashCode();
        }
    }
}
