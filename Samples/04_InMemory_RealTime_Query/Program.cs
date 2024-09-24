using Domain.Read;
using Framework.Impl.InMemory;

/*
 * This sample shows how the read model can be kept up-to-date in real time as events are added by the write side.
 */

// Events are saved in the EventStore.
var eventStore = new InMemoryEventStore();

var readRepository = new ProductReadRepository();

var writeTask = WriteSide.Execute(eventStore);
var readTask = ReadSide.Execute(eventStore, readRepository);
var queryTask = QuerySide.Execute(readRepository);

Task.WaitAll(writeTask, readTask, queryTask);

Console.WriteLine("DONE!");
