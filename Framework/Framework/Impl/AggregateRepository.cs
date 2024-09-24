using Framework.Core;

namespace Framework.Impl;

public class AggregateRepository<TAggregate>(IEventStore eventStore) : IAggregateRepository<TAggregate> 
    where TAggregate : IAggregateRoot, new()
{
    private readonly IEventStore eventStore = eventStore;

    // Load events from the Event Store and reconstruct the aggregate
    public async Task<TAggregate> GetById(Guid id)
    {
        var aggregate = new TAggregate() { Id = id };
        var eventStreamId = GetEventStreamId(aggregate);
        var events = await eventStore.GetEvents(eventStreamId);
        aggregate.LoadFromHistory(events);
        return aggregate;
    }

    // Save new events to the Event Store
    public void Save(TAggregate aggregate) 
    {
        var events = aggregate.GetUncommittedEvents();
        var eventStreamId = GetEventStreamId(aggregate);
        eventStore.AddEvents(eventStreamId, events);
        aggregate.MarkEventsAsCommitted();
    }

    private static string GetEventStreamId(TAggregate aggregate) => typeof(TAggregate).Name + "-" + aggregate.Id;
}