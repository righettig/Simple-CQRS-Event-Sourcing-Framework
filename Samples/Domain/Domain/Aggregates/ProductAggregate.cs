using Domain.Write.Events;
using Framework.Core;
using Framework.Impl;

namespace Domain.Aggregates;

public class ProductAggregate : AggregateRoot, IAggregateRoot
{
    // Only state that is relevant for business logic rules should be stored in the aggregate.
    // Also consider creating a class for the CurrentState.
    //public string Name { get; private set; }
    //public decimal Price { get; private set; }

    public void CreateProduct(Guid id, string name, decimal price)
    {
        RaiseEvent(new ProductCreatedEvent(id, name, price));
    }

    public void UpdatePrice(decimal price)
    {
        RaiseEvent(new ProductPriceUpdatedEvent(Id, price));
    }

    public void DeleteProduct(Guid id)
    {
        RaiseEvent(new ProductDeletedEvent(id));
    }

    // TODO: perhaps I should not be forced to process events if there are not relevant for my business logic
    private void Apply(ProductCreatedEvent @event)
    {
        //Name = @event.Name;
        //Price = @event.Price;
    }

    private void Apply(ProductPriceUpdatedEvent @event)
    {
        //Price = @event.Price;
    }

    private void Apply(ProductDeletedEvent @event)
    {
    }
}
