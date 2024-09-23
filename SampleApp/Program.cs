using Domain.Aggregates;
using Domain.Read;
using Domain.Read.Queries;
using Domain.Read.Queries.Handlers;
using Domain.Write.Commands;
using Domain.Write.Commands.Handlers;
using Domain.Write.Events;
using Domain.Write.Events.Handlers;
using Framework.Impl;

var eventStore = new InMemoryEventStore();

// This is how you listen for events as they happen in real time.
// Alternatively you can create the EventListener before any command is executed
//eventStore.Subscribe(async (streamId, events) =>
//{
//    foreach (var @event in events) 
//    {
//        Console.WriteLine($"Event received for stream {streamId}: {@event.GetType().Name}, occurred on {@event.CreatedAt}");
//        await Task.Delay(5000); // Simulate some asynchronous work
//        Console.WriteLine($"Completed processing event: {@event.CreatedAt}");
//    }
//});

//var settings = EventStoreClientSettings.Create("esdb://localhost:2113?tls=false");
//var eventStoreClient = new EventStoreClient(settings);
//var eventStore = new EventStoreDb(eventStoreClient);

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

// Keeping the read side up-to-date with the write side.
// This happens independently from the write side. The event store is the synchronisation point.
//eventStore.DumpEvents();

// This is how you can you iterate over the events even after they have been created
//await foreach (var (eventStreamId, @event) in eventStore.GetAllEventsAsync())
//{
//    Console.WriteLine($"Event Stream Id: {eventStreamId}, Event: {@event.GetType()}");
//}

// Read side
var readRepository = new ProductReadRepository();

var eventHandler1 = new ProductCreatedEventHandler(readRepository);
var eventHandler2 = new ProductPriceUpdatedEventHandler(readRepository);
var eventHandler3 = new ProductDeletedEventHandler(readRepository);

var eventListener = new EventListener<ProductReadModel>(readRepository);

eventListener.Bind<ProductCreatedEvent, ProductCreatedEventHandler>();
eventListener.Bind<ProductPriceUpdatedEvent, ProductPriceUpdatedEventHandler>();
eventListener.Bind<ProductDeletedEvent, ProductDeletedEventHandler>();

eventListener.ProcessEvents(eventStore);

// Queries
var q1 = new GetLowPricesProducts(50);
var q2 = new GetHighPricesProducts(50);

var queryHandler = new ProductQueryHandler(readRepository);
var result1 = await queryHandler.Handle(q1, CancellationToken.None);
var result2 = await queryHandler.Handle(q2, CancellationToken.None);

Console.WriteLine();

Console.WriteLine($"Query 1 results: {result1.Count()}");
Console.WriteLine($"Query 2 results: {result2.Count()}");
