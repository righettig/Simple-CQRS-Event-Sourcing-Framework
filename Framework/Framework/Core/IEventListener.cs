namespace Framework.Core;

public interface IEventListener
{
    void Bind<TEvent, THandler>() where THandler : IEventHandler<TEvent>;
    void SubscribeTo(IEventStore eventStore);
    Task ProcessEvents(IEventStore eventStore);
}