using Domain.Aggregates;
using Domain.Write.Commands;
using Domain.Write.Commands.Handlers;
using Framework.Core;
using Framework.Impl;

public static class WriteSide
{
    public static async Task Execute(IEventStore eventStore)
    {
        Console.WriteLine("Write side starting...");

        var aggregateRepository = new AggregateRepository<ProductAggregate>(eventStore);

        var handler1 = new CreateProductCommandHandler(aggregateRepository);
        var handler2 = new UpdateProductPriceCommandHandler(aggregateRepository);
        var handler3 = new DeleteProductCommandHandler(aggregateRepository);

        var command1 = new CreateProductCommand(Guid.NewGuid(), "product1", 100);
        var command2 = new UpdateProductPriceCommand(command1.Id, 200);
        var command3 = new CreateProductCommand(Guid.NewGuid(), "product2", 1000);
        var command4 = new DeleteProductCommand(command1.Id);

        await handler1.Handle(command1, CancellationToken.None);
        await Task.Delay(3000);

        await handler2.Handle(command2, CancellationToken.None);
        await Task.Delay(3000);

        await handler1.Handle(command3, CancellationToken.None);
        await Task.Delay(3000);

        await handler3.Handle(command4, CancellationToken.None);
        await Task.Delay(3000);

        Console.WriteLine("Write side has finished!");
    }
}
