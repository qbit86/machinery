namespace Machinery
{
    public interface IEventSource<in TEvent>
    {
        bool Process(TEvent ev);
    }
}
