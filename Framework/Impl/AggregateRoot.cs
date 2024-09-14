﻿using Framework.Core;
using System.Reflection;

namespace Framework.Impl;

public abstract class AggregateRoot : IAggregateRoot
{
    public Guid Id { get; init; }

    public IReadOnlyList<IEvent> GetUncommittedEvents() => Events.AsReadOnly();

    protected List<IEvent> Events { get; private set; } = [];

    public AggregateRoot()
    {
    }

    public void LoadFromHistory(IEnumerable<IEvent> events)
    {
        foreach (var @event in events)
        {
            ApplyEvent(@event);
        }
    }

    public void MarkEventsAsCommitted() => Events.Clear();

    protected void RaiseEvent(IEvent @event)
    {
        Events.Add(@event);
        ApplyEvent(@event);
    }

    private void ApplyEvent<TEvent>(TEvent @event)
    {
        // Use reflection to find and invoke the correct Apply method
        var applyMethod = GetType().GetMethod("Apply", BindingFlags.NonPublic | BindingFlags.Instance, [@event.GetType()]);

        if (applyMethod != null)
        {
            applyMethod.Invoke(this, [@event]);
        }
        else
        {
            throw new InvalidOperationException($"No Apply method found for event type {@event.GetType().Name}");
        }
    }
}