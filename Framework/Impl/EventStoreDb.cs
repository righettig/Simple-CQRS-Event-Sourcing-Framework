using EventStore.Client;
using Framework.Core;
using System.Text;
using System.Text.Json;

public class EventStoreDb : IEventStore
{
    private readonly EventStoreClient _client;

    public EventStoreDb(EventStoreClient client)
    {
        _client = client;
    }

    public async void AddEvents(string eventStreamId, IEnumerable<IEvent> events)
    {
        var eventDataList = events.Select(ToEventData).ToList();

        await _client.AppendToStreamAsync(eventStreamId, StreamState.Any, eventDataList);
    }

    public async Task<IReadOnlyCollection<IEvent>> GetEvents(string eventStreamId)
    {
        var events = new List<IEvent>();

        var stream = _client.ReadStreamAsync(Direction.Forwards, eventStreamId, StreamPosition.Start);

        if (await stream.ReadState != ReadState.StreamNotFound)
        {
            await foreach (var resolvedEvent in stream)
            {
                var eventData = Encoding.UTF8.GetString(resolvedEvent.Event.Data.Span);
                var eventType = Type.GetType(resolvedEvent.Event.EventType) ?? throw new Exception("Unknown event type");

                if (eventType != null)
                {
                    var @event = (IEvent)JsonSerializer.Deserialize(eventData, eventType)!;
                    events.Add(@event);
                }
            }
        };

        return events;
    }

    public async IAsyncEnumerable<(string eventStreamId, IEvent @event)> GetAllEventsAsync()
    {
        yield return ("", null);
    }

    public void Subscribe(Func<string, IEnumerable<IEvent>, Task> eventHandler) 
    {
        throw new NotImplementedException();
    }

    public async void DumpEvents() // Debug
    {
        var stream = _client.ReadAllAsync(Direction.Forwards, Position.Start);

        await foreach (var resolvedEvent in stream)
        {
            var eventType = resolvedEvent.Event.EventType;
            var json = Encoding.UTF8.GetString(resolvedEvent.Event.Data.Span);
            Console.WriteLine($"Event Type: {eventType}, Data: {json}");
        }
    }

    private static EventData ToEventData(IEvent @event)
    {
        var eventData = JsonSerializer.SerializeToUtf8Bytes(@event);
        var eventTypeName = @event.GetType().AssemblyQualifiedName;
        return new EventData(Uuid.NewUuid(), eventTypeName, eventData);
    }
}