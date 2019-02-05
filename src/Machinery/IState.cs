namespace Machinery
{
    using System;

    public interface IState<TStateBase, in TEvent, in TContext> : IDisposable
    {
        bool TryCreateNewState(TContext context, TEvent ev, out TStateBase newState);
        void OnExiting(TContext context, TEvent ev, TStateBase newState);
        void OnEntered(TContext context, TEvent ev, TStateBase oldState);
    }
}
