namespace Machinery
{
    using System;

#pragma warning disable CA1815, CA2231
    public readonly struct ContextBoundPolicy<TState, TEvent, TContext> : IPolicy<TState, TEvent>,
        IEquatable<ContextBoundPolicy<TState, TEvent, TContext>>
        where TState : IState<TState, TEvent, TContext>, IDisposable
#pragma warning restore CA2231, CA1815
    {
        private readonly TContext _context;

        public ContextBoundPolicy(TContext context)
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

        public bool Equals(ContextBoundPolicy<TState, TEvent, TContext> other)
        {
            return true;
        }

        public override bool Equals(object obj)
        {
            return obj is ContextBoundPolicy<TState, TEvent, TContext>;
        }

        public override int GetHashCode()
        {
            return typeof(ContextBoundPolicy<TState, TEvent, TContext>).GetHashCode();
        }
    }
}
