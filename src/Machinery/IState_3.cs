namespace Machinery
{
#if NETSTANDARD2_1 || NETCOREAPP3_0
    using System.Diagnostics.CodeAnalysis;

#endif

    public interface IState<in TContext, in TEvent, TState>
    {
#if NETSTANDARD2_1 || NETCOREAPP3_0
        bool TryCreateNewState(TContext context, TEvent ev, [MaybeNullWhen(false)] out TState newState);
#else
        bool TryCreateNewState(TContext context, TEvent ev, out TState newState);
#endif
        void OnExiting(TContext context, TEvent ev, TState newState);
        void OnRemain(TContext context, TEvent ev);
        void OnEntered(TContext context, TEvent ev, TState oldState);
    }
}
