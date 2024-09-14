using Framework.Core;

namespace Domain.Read.Queries;

public class GetLowPricesProducts(int price) : IQuery
{
    public int Price { get; } = price;
}