namespace Machinery
{
    public interface IStateMachine<in TEvent>
    {
        bool Process(TEvent ev);
    }
}
