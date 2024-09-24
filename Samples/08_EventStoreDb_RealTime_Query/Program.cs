using Domain.Read;
using EventStore.Client;
using Framework.Impl.EventStore;

/*
 * This sample shows how the read model can be kept up-to-date in real time as events are added by the write side.
 */

// Events are saved in the EventStore.
var settings = EventStoreClientSettings.Create("esdb://localhost:2113?tls=false");
var eventStoreClient = new EventStoreClient(settings);
var eventStore = new EventStoreDb(eventStoreClient);

var readRepository = new ProductReadRepository();

var writeTask = WriteSide.Execute(eventStore);
var readTask = ReadSide.Execute(eventStore, readRepository);
var queryTask = QuerySide.Execute(readRepository);

Task.WaitAll(writeTask, readTask, queryTask);

Console.WriteLine("DONE!");
