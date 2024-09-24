using Domain.Read;
using Domain.Read.Queries;
using Domain.Read.Queries.Handlers;

namespace _05_EventStoreDb_Batch_Reading_Events;

public static class QuerySide
{
    public static async Task Execute(ProductReadRepository readRepository)
    {
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
}