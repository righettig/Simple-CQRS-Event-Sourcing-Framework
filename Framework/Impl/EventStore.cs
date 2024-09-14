using Framework.Core;

namespace Framework.Impl;

public class EventStore : IEventStore
{
    private readonly Dictionary<Guid, List<IEvent>> events = [];

    // Define a delegate and event for event subscribers
    public delegate void EventsAddedHandler(IEnumerable<IEvent> events);
    public event EventsAddedHandler OnEventsAdded;

    public EventStore()
    {
    }

    public void AddEvents(Guid aggregateId, IEnumerable<IEvent> e)
    {
        if (!events.TryGetValue(aggregateId, out List<IEvent>? value))
        {
            value = ([]);
            events.Add(aggregateId, value);
        }

        value.AddRange(e);

        // Notify subscribers
        OnEventsAdded?.Invoke(e);
    }

    public IReadOnlyCollection<IEvent> GetEvents(Guid aggregateId)
    {
        events.TryGetValue(aggregateId, out var aggregateEvents);

        return (aggregateEvents ?? []).AsReadOnly();
    }

    public void DumpEvents() // Debug
    {
        foreach (var aggregateId in events.Keys)
        {
            events[aggregateId].ToList().ForEach(x => Console.WriteLine($"AggregateId: {aggregateId}, ${x.GetType()}"));
        }
    }
}