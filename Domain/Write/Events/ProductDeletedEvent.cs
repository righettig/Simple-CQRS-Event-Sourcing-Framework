﻿using Framework.Core;

namespace Domain.Write.Events;

public class ProductDeletedEvent(Guid id) : Event
{
    public Guid Id { get; } = id;
}
