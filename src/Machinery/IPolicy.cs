namespace Machinery
{
#if NETSTANDARD2_1 || NETCOREAPP3_0
    using System.Diagnostics.CodeAnalysis;

#endif

    public interface IPolicy<in TContext, in TEvent, TState>
    {
#if NETSTANDARD2_1 || NETCOREAPP3_0
        bool TryCreateNewState(TContext context, TEvent ev, TState currentState,
            [MaybeNullWhen(false)] out TState newState);
#else
        bool TryCreateNewState(TContext context, TEvent ev, TState currentState, out TState newState);
#endif
        void OnExiting(TContext context, TEvent ev, TState currentState, TState newState);
        void OnRemain(TContext context, TEvent ev, TState currentState);
        void OnEntered(TContext context, TEvent ev, TState currentState, TState oldState);
    }
}
