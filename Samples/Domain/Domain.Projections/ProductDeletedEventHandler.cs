using Domain.Events;
using Domain.Read;
using Framework.Core;
using Framework.Impl;

namespace Domain.Projections;

public class ProductDeletedEventHandler(IReadRepository<ProductReadModel> readRepository) :
    EventHandlerBase<ProductDeletedEvent, ProductReadModel>(readRepository), IEventHandler<ProductDeletedEvent>
{
    public override void Handle(ProductDeletedEvent @event)
    {
        readRepository.Remove(@event.Id);
        readRepository.SaveChanges();
    }
}