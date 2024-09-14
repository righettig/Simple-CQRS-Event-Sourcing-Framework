using Framework.Core;

namespace Framework.Impl;

public class EventListener<TReadModel> where TReadModel : class
{
    private readonly IReadRepository<TReadModel> _readRepository;

    private readonly Dictionary<Type, Action<object>> _handlers = [];

    public EventListener(IEventStore eventStore, IReadRepository<TReadModel> readRepository)
    {
        eventStore.OnEventsAdded += events =>
        {
            foreach (var e in events)
            {
                ExecuteHandlers(e);
            }

            // Dump data after processing events
            _readRepository.DumpData();
        };

        _readRepository = readRepository;
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