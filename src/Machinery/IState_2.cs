namespace Machinery
{
    using System.Diagnostics.CodeAnalysis;

    public interface IState<TContext, TEvent>
    {
        bool TryCreateNewState(TContext context, TEvent ev, [NotNullWhen(true)] out IState<TContext, TEvent>? newState);
        void OnExiting(TContext context, TEvent ev, IState<TContext, TEvent> newState);
        void OnRemain(TContext context, TEvent ev);
        void OnEntered(TContext context, TEvent ev, IState<TContext, TEvent> oldState);
    }
}
