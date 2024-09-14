using Framework.Core;

namespace Domain.Read.Queries.Handlers;

public class ProductQueryHandler(IReadRepository<ProductReadModel> readRepository) :
    IQueryHandler<GetLowPricesProducts, IEnumerable<ProductReadModel>>,
    IQueryHandler<GetHighPricesProducts, IEnumerable<ProductReadModel>>
{
    private readonly IReadRepository<ProductReadModel> readRepository = readRepository;

    public IEnumerable<ProductReadModel> Handle(GetLowPricesProducts query)
    {
        return readRepository.Entities.Where(x => x.Price <= query.Price);
    }

    public IEnumerable<ProductReadModel> Handle(GetHighPricesProducts query)
    {
        return readRepository.Entities.Where(x => x.Price > query.Price);
    }
}