namespace Machinery
{
#if NETSTANDARD2_1
    using System.Diagnostics.CodeAnalysis;

#endif

    public interface IState<TContext, TEvent>
    {
#if NETSTANDARD2_1
        bool TryCreateNewState(TContext context, TEvent ev, [NotNullWhen(true)] out IState<TContext, TEvent>? newState);
#else
        bool TryCreateNewState(TContext context, TEvent ev, out IState<TContext, TEvent> newState);
#endif
        void OnExiting(TContext context, TEvent ev, IState<TContext, TEvent> newState);
        void OnRemain(TContext context, TEvent ev);
        void OnEntered(TContext context, TEvent ev, IState<TContext, TEvent> oldState);
    }
}
