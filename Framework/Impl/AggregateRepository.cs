using Framework.Core;

namespace Framework.Impl;

public class AggregateRepository<TAggregate>(IEventStore eventStore) : IAggregateRepository<TAggregate> 
    where TAggregate : IAggregateRoot, new()
{
    private readonly IEventStore eventStore = eventStore;

    // Load events from the Event Store and reconstruct the aggregate
    public TAggregate GetById(Guid id)
    {
        var aggregate = new TAggregate() { Id = id };
        var events = eventStore.GetEvents(id);
        aggregate.LoadFromHistory(events);
        return aggregate;
    }

    // Save new events to the Event Store
    public void Save(TAggregate aggregate) 
    {
        var events = aggregate.GetUncommittedEvents();
        eventStore.AddEvents(aggregate.Id, events);
        aggregate.MarkEventsAsCommitted();
    }
}