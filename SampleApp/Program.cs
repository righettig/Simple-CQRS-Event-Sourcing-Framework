using Domain.Aggregates;
using Domain.Read;
using Domain.Read.Queries;
using Domain.Read.Queries.Handlers;
using Domain.Write.Commands;
using Domain.Write.Commands.Handlers;
using Domain.Write.Events;
using Domain.Write.Events.Handlers;
using Framework.Impl;

var eventStore = new EventStore();

// Read side
var readRepository = new ProductReadRepository();

var eventHandler1 = new ProductCreatedEventHandler(readRepository);
var eventHandler2 = new ProductPriceUpdatedEventHandler(readRepository);
var eventHandler3 = new ProductDeletedEventHandler(readRepository);

//eventStore.OnEventsAdded += (events) =>
//{
//    foreach (var e in events)
//    {
//        switch (e)
//        {
//            case ProductCreatedEvent @event:
//                eventHandler1.Handle(@event);
//                break;

//            case ProductPriceUpdatedEvent @event:
//                eventHandler2.Handle(@event);
//                break;

//            case ProductDeletedEvent @event:
//                eventHandler3.Handle(@event);
//                break;
//        }
//    }

//    readRepository.DumpData();
//};

var eventListener = new EventListener<ProductReadModel>(eventStore, readRepository);

eventListener.Bind<ProductCreatedEvent, ProductCreatedEventHandler>();
eventListener.Bind<ProductPriceUpdatedEvent, ProductPriceUpdatedEventHandler>();
eventListener.Bind<ProductDeletedEvent, ProductDeletedEventHandler>();

// Write side
var aggregateRepository = new AggregateRepository<ProductAggregate>(eventStore);

var handler1 = new CreateProductCommandHandler(aggregateRepository);
var handler2 = new UpdateProductPriceCommandHandler(aggregateRepository);
var handler3 = new DeleteProductCommandHandler(aggregateRepository);

var command1 = new CreateProductCommand(Guid.NewGuid(), "product1", 100);
var command2 = new UpdateProductPriceCommand(command1.Id, 200);
var command3 = new CreateProductCommand(Guid.NewGuid(), "product2", 1000);
var command4 = new DeleteProductCommand(command1.Id);

handler1.Handle(command1, CancellationToken.None);
handler2.Handle(command2, CancellationToken.None);
handler1.Handle(command3, CancellationToken.None);
handler3.Handle(command4, CancellationToken.None);

// Queries
var q1 = new GetLowPricesProducts(50);
var q2 = new GetHighPricesProducts(50);

var queryHandler = new ProductQueryHandler(readRepository);
var result1 = queryHandler.Handle(q1);
var result2 = queryHandler.Handle(q2);

Console.WriteLine($"Query 1 results: {result1.Count()}");
Console.WriteLine($"Query 2 results: {result2.Count()}");