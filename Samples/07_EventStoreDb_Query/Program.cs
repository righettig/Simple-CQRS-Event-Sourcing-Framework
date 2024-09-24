using Domain.Aggregates;
using Domain.Read;
using Domain.Read.Queries;
using Domain.Read.Queries.Handlers;
using Domain.Write.Commands;
using Domain.Write.Commands.Handlers;
using Domain.Write.Events;
using Domain.Write.Events.Handlers;
using EventStore.Client;
using Framework.Impl;
using Framework.Impl.EventStore;

/*
 * This sample shows how to build the read model after all commands have been processed by the write side.
 */

// Events are saved in the EventStore.
var settings = EventStoreClientSettings.Create("esdb://localhost:2113?tls=false");
var eventStoreClient = new EventStoreClient(settings);
var eventStore = new EventStoreDb(eventStoreClient);

// Write side
var aggregateRepository = new AggregateRepository<ProductAggregate>(eventStore);

var handler1 = new CreateProductCommandHandler(aggregateRepository);
var handler2 = new UpdateProductPriceCommandHandler(aggregateRepository);
var handler3 = new DeleteProductCommandHandler(aggregateRepository);

var command1 = new CreateProductCommand(Guid.NewGuid(), "product1", 100);
var command2 = new UpdateProductPriceCommand(command1.Id, 200);
var command3 = new CreateProductCommand(Guid.NewGuid(), "product2", 1000);
var command4 = new DeleteProductCommand(command1.Id);

await handler1.Handle(command1, CancellationToken.None);
await handler2.Handle(command2, CancellationToken.None);
await handler1.Handle(command3, CancellationToken.None);
await handler3.Handle(command4, CancellationToken.None);

// Read side
var readRepository = new ProductReadRepository();

var eventListener = new EventListener<ProductReadModel>(readRepository);

eventListener.Bind<ProductCreatedEvent, ProductCreatedEventHandler>();
eventListener.Bind<ProductPriceUpdatedEvent, ProductPriceUpdatedEventHandler>();
eventListener.Bind<ProductDeletedEvent, ProductDeletedEventHandler>();

await eventListener.ProcessEvents(eventStore);

// At this point the read repository is ready to be queried

// Queries
var q1 = new GetLowPricesProducts(50);
var q2 = new GetHighPricesProducts(50);

var queryHandler = new ProductQueryHandler(readRepository);
var result1 = await queryHandler.Handle(q1, CancellationToken.None);
var result2 = await queryHandler.Handle(q2, CancellationToken.None);

Console.WriteLine();

Console.WriteLine($"Query 1 results: {result1.Count()}");
Console.WriteLine($"Query 2 results: {result2.Count()}");

Console.ReadLine();