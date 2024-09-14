using Framework.Core;

namespace Domain.Write.Events;

public class ProductDeletedEvent(Guid id) : IEvent
{
    public Guid Id { get; } = id;
}
