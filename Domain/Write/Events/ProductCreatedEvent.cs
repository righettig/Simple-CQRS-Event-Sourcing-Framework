﻿using Framework.Core;

namespace Domain.Write.Events;

public class ProductCreatedEvent(Guid id, string name, decimal price) : IEvent
{
    public Guid Id { get; } = id;
    public string Name { get; } = name;
    public decimal Price { get; } = price;
}
