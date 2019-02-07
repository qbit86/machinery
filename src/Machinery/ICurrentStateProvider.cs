namespace Machinery
{
    public interface ICurrentStateProvider<out TState>
    {
        TState CurrentState { get; }
    }
}
