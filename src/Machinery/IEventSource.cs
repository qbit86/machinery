namespace Machinery
{
    public interface IEventSource<in TEvent>
    {
        bool ProcessEvent(TEvent ev);
    }
}
