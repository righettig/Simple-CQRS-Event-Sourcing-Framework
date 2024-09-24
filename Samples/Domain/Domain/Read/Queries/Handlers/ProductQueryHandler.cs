using Framework.Core;
using MediatR;

namespace Domain.Read.Queries.Handlers;

public class ProductQueryHandler(IReadRepository<ProductReadModel> readRepository) :
    IRequestHandler<GetLowPricesProducts, IEnumerable<ProductReadModel>>,
    IRequestHandler<GetHighPricesProducts, IEnumerable<ProductReadModel>>
{
    private readonly IReadRepository<ProductReadModel> readRepository = readRepository;

    public Task<IEnumerable<ProductReadModel>> Handle(GetLowPricesProducts query, CancellationToken cancellationToken)
    {
        var result = readRepository.Entities.Where(x => x.Price <= query.Price);

        return Task.FromResult(result.AsEnumerable());
    }

    public Task<IEnumerable<ProductReadModel>> Handle(GetHighPricesProducts query, CancellationToken cancellationToken)
    {
        var result = readRepository.Entities.Where(x => x.Price > query.Price);

        return Task.FromResult(result.AsEnumerable());
    }
}