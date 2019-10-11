// ReSharper disable SuspiciousTypeConversion.Global

namespace Machinery
{
    using System;
    using System.Threading;

    public static partial class StateMachine<TEvent>
    {
#pragma warning disable CA1000 // Do not declare static members on generic types
        public static StateMachine<TContext, TEvent, TState> Create<TContext, TState>(
            TContext context, TState initialState)
            where TState : IState<TContext, TEvent, TState>
        {
            return new StateMachine<TContext, TEvent, TState>(context, initialState);
        }
#pragma warning restore CA1000 // Do not declare static members on generic types
    }

#pragma warning disable CA1303 // Do not pass literals as localized parameters

    public sealed class StateMachine<TContext, TEvent, TState>
        where TState : IState<TContext, TEvent, TState>
    {
        private readonly TContext _context;

        private TState _currentState;
        private int _lock;

        public StateMachine(TContext context, TState initialState)
        {
            if (context is null)
                throw new ArgumentNullException(nameof(context));

            if (initialState is null)
                throw new ArgumentNullException(nameof(initialState));

            _context = context;
            _currentState = initialState;
        }

        public TState CurrentState => _currentState;

        public bool TryProcessEvent(TEvent ev)
        {
            if (Interlocked.Exchange(ref _lock, 1) == 1)
                return false;

            try
            {
                UncheckedProcessEvent(ev);
            }
            finally
            {
                Interlocked.Exchange(ref _lock, 0);
            }

            return true;
        }

        private void UncheckedProcessEvent(TEvent ev)
        {
            bool transit = _currentState.TryCreateNewState(_context, ev, out TState newState);
            if (!transit)
            {
                _currentState.OnRemain(_context, ev, _currentState);
                return;
            }

            if (newState is null)
                throw new InvalidOperationException("The new state must not be null.");

            try
            {
                _currentState.OnExiting(_context, ev, newState);
            }
            catch
            {
                (newState as IDisposable)?.Dispose();
                throw;
            }

            TState oldState = _currentState;
            _currentState = newState;

            try
            {
                _currentState.OnEntered(_context, ev, oldState);
            }
            finally
            {
                (oldState as IDisposable)?.Dispose();
            }
        }
    }
}