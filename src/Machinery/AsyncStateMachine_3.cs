namespace Machinery
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public static class AsyncStateMachine<TEvent>
    {
        public static AsyncStateMachine<TContext, TEvent, TState> Create<TContext, TState>(
            TContext context, TState initialState)
            where TState : IAsyncState<TContext, TEvent, TState> =>
            new(context, initialState);
    }

    public sealed class AsyncStateMachine<TContext, TEvent, TState>
        where TState : IAsyncState<TContext, TEvent, TState>
    {
        private readonly TContext _context;

        private TState _currentState;
        private int _lock;

        public AsyncStateMachine(TContext context, TState initialState)
        {
            if (initialState is null)
                throw new ArgumentNullException(nameof(initialState));

            _context = context;
            _currentState = initialState;
        }

        public TContext Context => _context;

        public TState CurrentState => _currentState;

        public async Task<bool> TryProcessEvent(TEvent ev)
        {
            if (Interlocked.Exchange(ref _lock, 1) != 0)
                return false;

            try
            {
                await UncheckedProcessEventAsync(ev).ConfigureAwait(false);
            }
            finally
            {
                _lock = 0;
            }

            return true;
        }

        private async Task UncheckedProcessEventAsync(TEvent ev)
        {
            bool transit = _currentState.TryCreateNewState(_context, ev, out TState? newState);
            if (!transit || newState is null)
            {
                await _currentState.OnRemainAsync(_context, ev).ConfigureAwait(false);
                return;
            }

            await _currentState.OnExitingAsync(_context, ev, newState).ConfigureAwait(false);
            await newState.OnEnteringAsync(_context, ev, _currentState).ConfigureAwait(false);

            TState oldState = _currentState;
            _currentState = newState;

            await oldState.OnExitedAsync(_context, ev, _currentState).ConfigureAwait(false);
            await _currentState.OnEnteredAsync(_context, ev, oldState).ConfigureAwait(false);
        }
    }
}
