namespace Machinery
{
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    public interface IAsyncState<in TContext, in TEvent, TState>
    {
        bool TryCreateNewState(TContext context, TEvent ev, [MaybeNullWhen(false)] out TState newState);
        Task OnExitingAsync(TContext context, TEvent ev, TState newState);
        Task OnExitedAsync(TContext context, TEvent ev, TState newState);
        Task OnRemainAsync(TContext context, TEvent ev);
        Task OnEnteringAsync(TContext context, TEvent ev, TState oldState);
        Task OnEnteredAsync(TContext context, TEvent ev, TState oldState);
    }
}
