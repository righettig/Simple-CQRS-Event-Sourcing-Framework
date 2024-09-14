using Framework.Impl;

namespace Framework.Core;

public interface IEventStore
{
    event EventStore.EventsAddedHandler OnEventsAdded;

    void AddEvents(Guid aggregateId, IEnumerable<IEvent> events);
    IReadOnlyCollection<IEvent> GetEvents(Guid aggregateId);
    
    void DumpEvents();
}