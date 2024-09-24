using Framework.Core;

namespace Framework.Impl;

public abstract class EventHandlerBase<TEvent, TReadModel>(IReadRepository<TReadModel> readRepository)
    where TEvent : IEvent
    where TReadModel : class
{
    protected IReadRepository<TReadModel> readRepository = readRepository;

    public abstract void Handle(TEvent @event);
}