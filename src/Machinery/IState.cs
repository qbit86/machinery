namespace Machinery
{
    using System;

    public interface IState<in TContext, TState, in TEvent> : IDisposable
    {
        bool TryCreateNewState(TContext context, TEvent ev, out TState newState);
        void OnExiting(TContext context, TEvent ev, TState newState);
        void OnEntered(TContext context, TEvent ev, TState oldState);
    }
}
