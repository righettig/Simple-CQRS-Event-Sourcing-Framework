using Framework.Core;

namespace Domain.Read.Queries;

public class GetLowPricesProducts(int price) : IQuery<IEnumerable<ProductReadModel>>
{
    public int Price { get; } = price;
}