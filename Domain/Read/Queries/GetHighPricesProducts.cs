using Framework.Core;

namespace Domain.Read.Queries;

public class GetHighPricesProducts(int price) : IQuery
{
    public int Price { get; } = price;
}