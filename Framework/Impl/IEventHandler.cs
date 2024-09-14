namespace Framework.Impl;

public interface IEventHandler<TEvent> 
{
    public void Handle(TEvent @event);
}
