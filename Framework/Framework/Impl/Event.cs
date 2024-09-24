using Framework.Core;

namespace Framework.Impl;

public abstract class Event : IEvent
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}