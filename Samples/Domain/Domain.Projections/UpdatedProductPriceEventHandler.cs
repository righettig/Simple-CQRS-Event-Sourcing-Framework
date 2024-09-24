using Domain.Events;
using Domain.Read;
using Framework.Core;
using Framework.Impl;

namespace Domain.Projections;

public class ProductPriceUpdatedEventHandler(IReadRepository<ProductReadModel> readRepository) :
    EventHandlerBase<ProductPriceUpdatedEvent, ProductReadModel>(readRepository), IEventHandler<ProductPriceUpdatedEvent>
{
    public override void Handle(ProductPriceUpdatedEvent @event)
    {
        var model = readRepository.GetById(@event.Id);

        if (model != null)
        {
            model.Price = @event.Price;

            readRepository.Update(model);
            readRepository.SaveChanges();
        }
    }
}