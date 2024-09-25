using Framework.Core;

namespace Framework.Impl;

public class EventListener<TReadModel> : IEventListener where TReadModel : class
{
    private readonly IReadRepository<TReadModel> _readRepository;

    private readonly Dictionary<Type, Action<object>> _handlers = [];

    public EventListener(IReadRepository<TReadModel> readRepository)
    {
        _readRepository = readRepository;
    }

    // this is using GetAllEventsAsync which does not use server-side filtering when retrieving events.
    [Obsolete] 
    public async Task ProcessEvents(IEventStore eventStore, string prefix = "") 
    {
        await foreach (var (eventStreamId, @event) in eventStore.GetAllEventsAsync(prefix))
        {
            Console.WriteLine($"Event Stream Id: {eventStreamId}, Event: {@event.GetType()}");

            ExecuteHandlers(@event);
        }
    }

    public void SubscribeTo(IEventStore eventStore, string prefix = "")
    {
        eventStore.Subscribe(async (streamId, events) =>
        {
            foreach (var @event in events)
            {
                Console.WriteLine($"Event Stream Id: {streamId}, Event: {@event.GetType()}");

                ExecuteHandlers(@event);
            }
        }, prefix);
    }

    public void Bind<TEvent, THandler>() where THandler : IEventHandler<TEvent>
    {
        var eventType = typeof(TEvent);

        if (!_handlers.ContainsKey(eventType))
        {
            var handlerInstance = (THandler)Activator.CreateInstance(typeof(THandler), _readRepository);
            _handlers[eventType] = e => handlerInstance.Handle((TEvent)e);
        }
    }

    private void ExecuteHandlers(IEvent @event)
    {
        var eventType = @event.GetType();

        if (_handlers.TryGetValue(eventType, out Action<object>? value))
        {
            value(@event);
        }
    }
}