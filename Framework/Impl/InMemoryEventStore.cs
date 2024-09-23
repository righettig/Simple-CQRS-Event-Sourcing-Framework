using Framework.Core;

namespace Framework.Impl;

public class InMemoryEventStore : IEventStore
{
    private readonly Dictionary<string, List<IEvent>> events = [];
    
    private readonly List<Func<string, IEnumerable<IEvent>, Task>> subscribers = [];

    public InMemoryEventStore()
    {
    }

    public void AddEvents(string eventStreamId, IEnumerable<IEvent> e)
    {
        if (!events.TryGetValue(eventStreamId, out List<IEvent>? value))
        {
            value = ([]);
            events.Add(eventStreamId, value);
        }

        var _events = e.ToArray(); // otherwise "e" is cleared out when executing MarkEventsAsCommitted.
        value.AddRange(_events);

        Task.Run(() => NotifySubscribersAsync(eventStreamId, _events));
    }

    public Task<IReadOnlyCollection<IEvent>> GetEvents(string eventStreamId)
    {
        events.TryGetValue(eventStreamId, out var aggregateEvents);

        IReadOnlyCollection<IEvent> result = (aggregateEvents ?? []).AsReadOnly();

        return Task.FromResult(result);
    }

    public async IAsyncEnumerable<(string eventStreamId, IEvent @event)> GetAllEventsAsync()
    {
        foreach (var eventStream in events)
        {
            foreach (var eventItem in eventStream.Value)
            {
                yield return (eventStream.Key, eventItem);
            }

            // Simulate asynchronous behavior
            await Task.Yield();
        }
    }

    public void Subscribe(Func<string, IEnumerable<IEvent>, Task> eventHandler)
    {
        subscribers.Add(eventHandler);
    }   

    public void DumpEvents() // Debug
    {
        foreach (var eventStreamId in events.Keys)
        {
            events[eventStreamId].ToList().ForEach(x => Console.WriteLine($"Event Stream Id: {eventStreamId}, ${x.GetType()}"));
        }
    }

    private async Task NotifySubscribersAsync(string streamId, IEnumerable<IEvent> events)
    {
        var tasks = subscribers.Select(subscriber => subscriber(streamId, events));
        await Task.WhenAll(tasks);
    }
}