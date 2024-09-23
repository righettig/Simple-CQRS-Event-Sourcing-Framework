﻿using Framework.Core;

namespace Domain.Write.Events;

public class ProductPriceUpdatedEvent(Guid id, decimal price) : Event
{
    public Guid Id { get; } = id;
    public decimal Price { get; } = price;
}
