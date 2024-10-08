﻿namespace Framework.Core;

public interface IAggregateRoot
{
    Guid Id { get; init; }
    IReadOnlyList<IEvent> GetUncommittedEvents();
    void MarkEventsAsCommitted();
    void LoadFromHistory(IEnumerable<IEvent> events);
}
