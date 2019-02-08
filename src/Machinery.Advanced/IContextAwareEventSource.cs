namespace Machinery
{
    public interface IContextAwareEventSource<in TEvent, in TContext>
    {
        bool Process(TContext context, TEvent ev);
    }
}
