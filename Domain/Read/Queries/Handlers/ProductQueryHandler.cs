using Framework.Core;

namespace Domain.Read.Queries.Handlers;

public class ProductQueryHandler(ProductReadRepository readRepository) :
    IQueryHandler<GetLowPricesProducts, IEnumerable<ProductReadModel>>,
    IQueryHandler<GetHighPricesProducts, IEnumerable<ProductReadModel>>
{
    private readonly ProductReadRepository readRepository = readRepository;

    public IEnumerable<ProductReadModel> Handle(GetLowPricesProducts query)
    {
        return readRepository.Products.Where(x => x.Price <= query.Price);
    }

    public IEnumerable<ProductReadModel> Handle(GetHighPricesProducts query)
    {
        return readRepository.Products.Where(x => x.Price > query.Price);
    }
}