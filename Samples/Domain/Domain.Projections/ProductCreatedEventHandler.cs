using Domain.Events;
using Domain.Read;
using Framework.Core;
using Framework.Impl;

namespace Domain.Projections;

public class ProductCreatedEventHandler(IReadRepository<ProductReadModel> readRepository) :
    EventHandlerBase<ProductCreatedEvent, ProductReadModel>(readRepository), IEventHandler<ProductCreatedEvent>
{
    public override void Handle(ProductCreatedEvent @event)
    {
        readRepository.Add(new ProductReadModel { Id = @event.Id, Name = @event.Name, Price = @event.Price });
        readRepository.SaveChanges();
    }
}