namespace Framework.Core;

public interface IEventHandler<TEvent>
{
    public void Handle(TEvent @event);
}
