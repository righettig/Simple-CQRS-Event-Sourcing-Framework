using Framework.Core;

namespace Domain.Read;

public class ProductReadRepository : IReadRepository<ProductReadModel>
{
    private readonly List<ProductReadModel> products = [];

    public IQueryable<ProductReadModel> Products => products.AsQueryable();

    public void Add(ProductReadModel productReadModel)
    {
        products.Add(productReadModel);
    }

    public ProductReadModel GetById(Guid id)
    {
        return products.FirstOrDefault(x => x.Id == id);
    }

    public void Update(ProductReadModel model)
    {
        var index = products.FindIndex(x => x.Id == model.Id);
        products[index] = model;
    }

    public void Remove(Guid id)
    {
        var index = products.FindIndex(x => x.Id == id);
        products.RemoveAt(index);
    }

    public void SaveChanges()
    {
        Console.WriteLine("Saving changes");
    }

    public void DumpData()
    {
        products.ToList().ForEach(Console.WriteLine);
    }
}