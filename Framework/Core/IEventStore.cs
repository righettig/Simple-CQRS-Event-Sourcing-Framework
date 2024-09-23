namespace Framework.Core;

public interface IEventStore
{
    void AddEvents(string eventStreamId, IEnumerable<IEvent> events);
    Task<IReadOnlyCollection<IEvent>> GetEvents(string eventStreamId);
    void Subscribe(Func<string, IEnumerable<IEvent>, Task> eventHandler);
    IAsyncEnumerable<(string eventStreamId, IEvent @event)> GetAllEventsAsync();
    void DumpEvents();
}