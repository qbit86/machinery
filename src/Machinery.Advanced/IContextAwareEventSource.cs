namespace Machinery
{
    public interface IContextAwareEventSource<in TEvent, in TContext>
    {
        bool ProcessEvent(TContext context, TEvent ev);
    }
}
