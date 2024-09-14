using Domain.Read;
using Framework.Core;
using Framework.Impl;

namespace Domain.Write.Events.Handlers;

public class ProductDeletedEventHandler(IReadRepository<ProductReadModel> readRepository) :
    EventHandlerBase<ProductDeletedEvent, ProductReadModel>(readRepository)
{
    public override void Handle(ProductDeletedEvent @event)
    {
        readRepository.Remove(@event.Id);
        readRepository.SaveChanges();
    }
}