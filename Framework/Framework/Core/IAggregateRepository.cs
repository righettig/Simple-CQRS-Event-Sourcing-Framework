namespace Framework.Core;

public interface IAggregateRepository<TAggregate> where TAggregate : IAggregateRoot
{
    Task<TAggregate> GetById(Guid id);
    void Save(TAggregate aggregate);
}