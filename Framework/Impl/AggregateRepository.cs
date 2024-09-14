using Framework.Core;

namespace Framework.Impl;

public class AggregateRepository<TAggregate>(IEventStore eventStore) : IAggregateRepository<TAggregate> where TAggregate : IAggregateRoot
{
    private readonly IEventStore eventStore = eventStore;
    private readonly Dictionary<Guid, TAggregate> products = [];

    public TAggregate FindById(Guid id)
    {
        products.TryGetValue(id, out TAggregate result);
        return result;
    }

    public void Remove(Guid id)
    {
        products.Remove(id);
    }

    public void Add(TAggregate aggregate)
    {
        products[aggregate.Id] = aggregate;
    }

    public void AddEvents(Guid id, IReadOnlyList<IEvent> events)
    {
        eventStore.AddEvents(id, events);
    }
}