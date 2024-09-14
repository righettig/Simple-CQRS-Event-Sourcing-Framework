namespace Framework.Core;

public interface IAggregateRepository<TAggregate> where TAggregate : IAggregateRoot
{
    TAggregate GetById(Guid id);
    void Save(TAggregate aggregate);
}