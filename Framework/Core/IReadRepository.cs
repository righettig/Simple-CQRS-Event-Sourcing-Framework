namespace Framework.Core;

public interface IReadRepository<TReadModel> where TReadModel : class
{
    void Add(TReadModel model);
    void DumpData();
    TReadModel GetById(Guid id);
    void Remove(Guid id);
    void SaveChanges();
    void Update(TReadModel model);
    IQueryable<TReadModel> Entities { get; }
}
