namespace Framework.Core;

public interface IReadRepository<TReadModel> where TReadModel : class
{
    IQueryable<TReadModel> Entities { get; }
    void Add(TReadModel model);
    TReadModel GetById(Guid id);
    void Update(TReadModel model);
    void Remove(Guid id);
    void SaveChanges();
    void DumpData();
}
