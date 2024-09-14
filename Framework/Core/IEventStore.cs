using Framework.Impl;

namespace Framework.Core;

public interface IEventStore
{
    event EventStore.EventsAddedHandler OnEventsAdded;

    void AddEvents(Guid aggregateId, IEnumerable<IEvent> events);
    void DumpEvents();
    IEnumerable<IEvent> GetEvents(Guid aggregateId);
}