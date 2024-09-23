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

    public async void ProcessEvents(IEventStore eventStore) 
    {
        await foreach (var (eventStreamId, @event) in eventStore.GetAllEventsAsync())
        {
            Console.WriteLine($"Event Stream Id: {eventStreamId}, Event: {@event.GetType()}");

            ExecuteHandlers(@event);
        }
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