using Domain.Aggregates;
using Framework.Impl;

namespace Domain.Write.Commands.Handlers;

public class UpdateProductPriceCommandHandler(AggregateRepository<ProductAggregate> aggregateRepository)
    : CommandHandlerBase<UpdateProductPriceCommand, ProductAggregate>(aggregateRepository)
{
    protected override Guid GetAggregateId(UpdateProductPriceCommand command) => command.Id;

    protected override void ProcessCommand(UpdateProductPriceCommand command, ProductAggregate aggregate)
    {
        aggregate.UpdatePrice(command.Price);
    }
}