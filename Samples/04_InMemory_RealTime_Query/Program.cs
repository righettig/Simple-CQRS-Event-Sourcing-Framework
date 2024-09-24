using Domain.Aggregates;
using Domain.Read;
using Domain.Read.Queries;
using Domain.Read.Queries.Handlers;
using Domain.Write.Commands;
using Domain.Write.Commands.Handlers;
using Domain.Write.Events;
using Domain.Write.Events.Handlers;
using Framework.Impl;
using Framework.Impl.InMemory;

/*
 * This sample shows how the read model can be kept up-to-date in real time as events are added by the write side.
 */

// Events are saved in the EventStore.
var eventStore = new InMemoryEventStore();

var readRepository = new ProductReadRepository();

// Write side
var write = Task.Run(
    async () => {
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
);

// Read side
var read = Task.Run(
    () => {
        Console.WriteLine("Read side is now processing events");

        var eventListener = new EventListener<ProductReadModel>(readRepository);

        eventListener.Bind<ProductCreatedEvent, ProductCreatedEventHandler>();
        eventListener.Bind<ProductPriceUpdatedEvent, ProductPriceUpdatedEventHandler>();
        eventListener.Bind<ProductDeletedEvent, ProductDeletedEventHandler>();

        eventListener.SubscribeTo(eventStore);
    }
);

// Query side
var query = Task.Run(
    async () => {
        Console.WriteLine("Query side starting...");

        var q1 = new GetLowPricesProducts(50);
        var q2 = new GetHighPricesProducts(50);

        var queryHandler = new ProductQueryHandler(readRepository);

        // Run the queries immediately
        var result1 = await queryHandler.Handle(q1, CancellationToken.None);
        var result2 = await queryHandler.Handle(q2, CancellationToken.None);
        
        Console.WriteLine($"Query 1 results: {result1.Count()}");
        Console.WriteLine($"Query 2 results: {result2.Count()}");

        // Retry after 2 sec
        await Task.Delay(2000);

        var result3 = await queryHandler.Handle(q1, CancellationToken.None);
        var result4 = await queryHandler.Handle(q2, CancellationToken.None);

        Console.WriteLine($"Query 1 results: {result3.Count()}");
        Console.WriteLine($"Query 2 results: {result4.Count()}");

        // Retry after 2 more sec
        await Task.Delay(2000);

        var result5 = await queryHandler.Handle(q1, CancellationToken.None);
        var result6 = await queryHandler.Handle(q2, CancellationToken.None);

        Console.WriteLine($"Query 1 results: {result5.Count()}");
        Console.WriteLine($"Query 2 results: {result6.Count()}");

        // Retry after 2 more sec
        await Task.Delay(2000);

        var result7 = await queryHandler.Handle(q1, CancellationToken.None);
        var result8 = await queryHandler.Handle(q2, CancellationToken.None);

        Console.WriteLine($"Query 1 results: {result7.Count()}");
        Console.WriteLine($"Query 2 results: {result8.Count()}");

        // Retry after 3 more sec
        await Task.Delay(3000);

        var result9 = await queryHandler.Handle(q1, CancellationToken.None);
        var result10 = await queryHandler.Handle(q2, CancellationToken.None);

        Console.WriteLine($"Query 1 results: {result9.Count()}");
        Console.WriteLine($"Query 2 results: {result10.Count()}");

        Console.WriteLine("Query side has finished!");
    }
);

Task.WaitAll(write, read, query);

Console.WriteLine("DONE!");
