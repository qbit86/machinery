namespace Machinery
{
    using System;

#pragma warning disable CA1815, CA2231
    public readonly struct ContextStatePolicy<TState, TEvent, TContext> : IStatePolicy<TState, TEvent>,
        IEquatable<ContextStatePolicy<TState, TEvent, TContext>>
        where TState : IState<TState, TEvent, TContext>
#pragma warning restore CA2231, CA1815
    {
        private readonly TContext _context;

        public ContextStatePolicy(TContext context)
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

        public void DisposeState(TState currentState, TEvent ev, TState stateToDispose)
        {
            currentState.Dispose();
        }

        public bool Equals(ContextStatePolicy<TState, TEvent, TContext> other)
        {
            return true;
        }

        public override bool Equals(object obj)
        {
            return obj is ContextStatePolicy<TState, TEvent, TContext>;
        }

        public override int GetHashCode()
        {
            return typeof(ContextStatePolicy<TState, TEvent, TContext>).GetHashCode();
        }
    }
}
