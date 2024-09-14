using Domain.Aggregates;
using Framework.Impl;

namespace Domain.Write.Commands.Handlers;

public class DeleteProductCommandHandler(AggregateRepository<ProductAggregate> aggregateRepository)
    : CommandHandlerBase<DeleteProductCommand, ProductAggregate>(aggregateRepository)
{
    protected override void ProcessCommand(DeleteProductCommand command, ProductAggregate aggregate)
    {
        aggregate.DeleteProduct(command.Id);

        // TODO: do I need to delete the aggregate from the aggregate repository?
    }
}