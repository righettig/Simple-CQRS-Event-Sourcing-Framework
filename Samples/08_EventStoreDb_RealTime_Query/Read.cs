using Domain.Read;
using Domain.Write.Events;
using Domain.Write.Events.Handlers;
using Framework.Core;
using Framework.Impl;

public static class ReadSide
{
    public static Task Execute(IEventStore eventStore, ProductReadRepository readRepository)
    {
        return Task.Run(() => 
        {
            Console.WriteLine("Read side is now processing events");

            var eventListener = new EventListener<ProductReadModel>(readRepository);

            eventListener.Bind<ProductCreatedEvent, ProductCreatedEventHandler>();
            eventListener.Bind<ProductPriceUpdatedEvent, ProductPriceUpdatedEventHandler>();
            eventListener.Bind<ProductDeletedEvent, ProductDeletedEventHandler>();

            eventListener.SubscribeTo(eventStore);
        });
    }
}
