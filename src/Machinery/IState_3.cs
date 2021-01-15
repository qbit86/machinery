namespace Machinery
{
    using System.Diagnostics.CodeAnalysis;

    public interface IState<in TContext, in TEvent, TState>
    {
        bool TryCreateNewState(TContext context, TEvent ev, [MaybeNullWhen(false)] out TState newState);
        void OnExiting(TContext context, TEvent ev, TState newState);
        void OnRemain(TContext context, TEvent ev);
        void OnEntered(TContext context, TEvent ev, TState oldState);
    }
}
