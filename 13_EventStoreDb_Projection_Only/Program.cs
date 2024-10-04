using Domain.Events;
using Domain.Projections;
using Domain.Read;
using EventStore.Client;
using Framework.Impl;
using Framework.Impl.EventStore;

/**
 * This sample shows how a single console app can be used to perform projections using the EventStoreDb.
 */

// TODO: need this to write to a real db so that the read-side can just import the ProductReadRepository 
// and serve the data from the controllers

var settings = EventStoreClientSettings.Create("esdb://localhost:2113?tls=false");
var eventStoreClient = new EventStoreClient(settings);
var eventStore = new EventStoreDb(eventStoreClient);

var readRepository = new ProductReadRepository();

var eventListener = new EventListener<ProductReadModel>(readRepository);

eventListener.Bind<ProductCreatedEvent,      ProductCreatedEventHandler>();
eventListener.Bind<ProductPriceUpdatedEvent, ProductPriceUpdatedEventHandler>();
eventListener.Bind<ProductDeletedEvent,      ProductDeletedEventHandler>();

eventListener.SubscribeTo(eventStore, "Domain.Events");

Console.ReadLine();
