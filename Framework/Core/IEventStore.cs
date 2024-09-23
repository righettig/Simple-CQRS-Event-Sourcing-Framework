using Framework.Impl;

namespace Framework.Core;

public delegate void EventsAddedHandler(IEnumerable<IEvent> events);

public interface IEventStore
{
    event EventsAddedHandler OnEventsAdded;

    void AddEvents(Guid aggregateId, IEnumerable<IEvent> events);
    IReadOnlyCollection<IEvent> GetEvents(Guid aggregateId);
    
    void DumpEvents();
}