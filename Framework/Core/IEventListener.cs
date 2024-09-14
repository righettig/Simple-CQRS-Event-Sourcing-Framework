namespace Framework.Core;

public interface IEventListener
{
    void Bind<TEvent, THandler>() where THandler : IEventHandler<TEvent>;
}