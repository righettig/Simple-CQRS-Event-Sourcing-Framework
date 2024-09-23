namespace Framework.Core;

public abstract class Event : IEvent
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}