using Framework.Core;

namespace Domain.Read.Queries;

public class GetHighPricesProducts(int price) : IQuery<IEnumerable<ProductReadModel>>
{
    public int Price { get; } = price;
}