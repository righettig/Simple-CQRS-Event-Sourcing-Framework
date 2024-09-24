using Domain.Aggregates;
using Domain.Write.Commands;
using Domain.Write.Commands.Handlers;
using EventStore.Client;
using Framework.Impl;
using Framework.Impl.EventStore;

/**
 * This sample shows how events can be read in real time as they are been processed by the command handlers.
 */

// Events are saved in the EventStore.
var settings = EventStoreClientSettings.Create("esdb://localhost:2113?tls=false");
var eventStoreClient = new EventStoreClient(settings);
var eventStore = new EventStoreDb(eventStoreClient);

// This is how you can subscribe for events as they happen in real time.
// This code is NOT executed on the main thread the write logic is executed on.
// This is why there is a Console.ReadLine at the bottom to allow the other threads to execute this.
eventStore.Subscribe(async (streamId, events) =>
{
    foreach (var @event in events)
    {
        Console.WriteLine($"Event Stream Id: {streamId}, Event: {@event.GetType()}");
    }
});

var aggregateRepository = new AggregateRepository<ProductAggregate>(eventStore);

var handler1 = new CreateProductCommandHandler(aggregateRepository);
var handler2 = new UpdateProductPriceCommandHandler(aggregateRepository);
var handler3 = new DeleteProductCommandHandler(aggregateRepository);

var command1 = new CreateProductCommand(Guid.NewGuid(), "product1", 100);
var command2 = new UpdateProductPriceCommand(command1.Id, 200);
var command3 = new CreateProductCommand(Guid.NewGuid(), "product2", 1000);
var command4 = new DeleteProductCommand(command1.Id);

await handler1.Handle(command1, CancellationToken.None);
await Task.Delay(1000);

await handler2.Handle(command2, CancellationToken.None);
await Task.Delay(1000);

await handler1.Handle(command3, CancellationToken.None);
await Task.Delay(1000);

await handler3.Handle(command4, CancellationToken.None);
await Task.Delay(1000);

Console.ReadLine();
