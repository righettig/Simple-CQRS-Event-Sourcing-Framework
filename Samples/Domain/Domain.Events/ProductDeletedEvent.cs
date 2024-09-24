using Framework.Impl;

namespace Domain.Events;

public class ProductDeletedEvent(Guid id) : Event
{
    public Guid Id { get; } = id;
}
