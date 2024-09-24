using EventStore.Client;
using Framework.Core;
using System.Text;
using System.Text.Json;

namespace Framework.Impl.EventStore;

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

        return events.AsReadOnly();
    }

    public async IAsyncEnumerable<(string eventStreamId, IEvent @event)> GetAllEventsAsync()
    {
        // TODO: this returns ALL events. I need to filter the events to get only those created by the framework.
        var stream = _client.ReadAllAsync(Direction.Forwards, Position.Start);

        //var stream = _client
        //    .ReadAllAsync(Direction.Forwards, Position.Start)
        //    .Where(@event => @event.OriginalStreamId.StartsWith("ProductAggregate-"));

        await foreach (var resolvedEvent in stream)
        {
            if (!resolvedEvent.Event.EventType.StartsWith("$")) // skip "metadata" streams
            {
                var eventType = Type.GetType(resolvedEvent.Event.EventType) ?? throw new Exception("Unknown event type");
                var eventData = Encoding.UTF8.GetString(resolvedEvent.Event.Data.Span);
                var @event = (IEvent)JsonSerializer.Deserialize(eventData, eventType)!;

                yield return (resolvedEvent.Event.EventStreamId, @event);

                // Simulate asynchronous behavior
                await Task.Yield();
            }
        }
    }

    public void Subscribe(Func<string, IEnumerable<IEvent>, Task> eventHandler)
    {
        // TODO: this returns ALL events. I need to filter the events to get only those created by the framework.

        // Create a persistent subscription to the stream for notifications
        var subscription = _client.SubscribeToAllAsync(
            FromAll.Start,
            async (subscription, resolvedEvent, cancellationToken) =>
            {
                if (resolvedEvent.Event.EventType.StartsWith("$")) // skip "metadata" streams
                {
                    return;
                }

                var eventType = Type.GetType(resolvedEvent.Event.EventType) ?? throw new Exception("Unknown event type");
                var eventData = Encoding.UTF8.GetString(resolvedEvent.Event.Data.Span);
                var @event = (IEvent)JsonSerializer.Deserialize(eventData, eventType)!;

                await eventHandler(resolvedEvent.Event.EventStreamId, new[] { @event });
            });

        // For simplicity, you could maintain the subscription object if needed
    }

    public async void DumpEvents(string query = "") // Debug
    {
        //var stream = _client.ReadAllAsync(Direction.Forwards, Position.Start);

        var stream = _client
            .ReadAllAsync(Direction.Forwards, Position.Start)
            .Where(@event => @event.OriginalStreamId.StartsWith(query));

        await foreach (var resolvedEvent in stream)
        {
            if (!resolvedEvent.Event.EventType.StartsWith("$")) // skip "metadata" streams
            {
                var eventType = resolvedEvent.Event.EventType;
                var json = Encoding.UTF8.GetString(resolvedEvent.Event.Data.Span);
                Console.WriteLine($"Stream Id: {resolvedEvent.OriginalStreamId}, Event Type: {eventType}, Data: {json}");
            }
        }
    }

    private static EventData ToEventData(IEvent @event)
    {
        // Casting to object allows all properties of derived classes to be serialised correctly
        // https://stackoverflow.com/questions/65664086/system-text-json-serialize-derived-class-property
        var eventData = JsonSerializer.SerializeToUtf8Bytes((object)@event);

        var eventTypeName = @event.GetType().AssemblyQualifiedName;

        return new EventData(Uuid.NewUuid(), eventTypeName, eventData);
    }
}