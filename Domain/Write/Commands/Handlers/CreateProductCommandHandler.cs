using Domain.Aggregates;
using Framework.Impl;

namespace Domain.Write.Commands.Handlers;

public class CreateProductCommandHandler(AggregateRepository<ProductAggregate> aggregateRepository)
    : CommandHandlerBase<CreateProductCommand, ProductAggregate>(aggregateRepository), ICommandHandler<CreateProductCommand>
{
    protected override void ProcessCommand(CreateProductCommand command, ProductAggregate aggregate)
    {
        aggregate.CreateProduct(command.Id, command.Name, command.Price);
    }
}