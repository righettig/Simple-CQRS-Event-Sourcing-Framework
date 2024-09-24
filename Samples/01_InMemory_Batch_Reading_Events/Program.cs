using Domain.Aggregates;
using Domain.Write.Commands;
using Domain.Write.Commands.Handlers;
using Framework.Impl;
using Framework.Impl.InMemory;

/** 
 * This sample shows how events can be read in batch after they have been written in the EventStore.
 */

// Events are saved in the EventStore.
var eventStore = new InMemoryEventStore();

var aggregateRepository = new AggregateRepository<ProductAggregate>(eventStore);

var handler1 = new CreateProductCommandHandler(aggregateRepository);
var handler2 = new UpdateProductPriceCommandHandler(aggregateRepository);
var handler3 = new DeleteProductCommandHandler(aggregateRepository);

var command1 = new CreateProductCommand(Guid.NewGuid(), "product1", 100);
var command2 = new UpdateProductPriceCommand(command1.Id, 200);
var command3 = new CreateProductCommand(Guid.NewGuid(), "product2", 1000);
var command4 = new DeleteProductCommand(command1.Id);

// Each execution of the Handle method produces one or more events that are written in the EventStore.
await handler1.Handle(command1, CancellationToken.None);
await handler2.Handle(command2, CancellationToken.None);
await handler1.Handle(command3, CancellationToken.None);
await handler3.Handle(command4, CancellationToken.None);

// This is how you can retrieve the events reading them back from the EventStore.
await foreach (var (eventStreamId, @event) in eventStore.GetAllEventsAsync())
{
    Console.WriteLine($"Event Stream Id: {eventStreamId}, Event: {@event.GetType()}");
}
