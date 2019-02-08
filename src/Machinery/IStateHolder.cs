namespace Machinery
{
    public interface IStateHolder<out TState>
    {
        TState CurrentState { get; }
    }
}
