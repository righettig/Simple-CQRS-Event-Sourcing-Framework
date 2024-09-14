using Framework.Core;

namespace Framework.Impl;

public abstract class CommandHandlerBase<TCommand, TAggregate>
    where TCommand : ICommand
    where TAggregate : IAggregateRoot, new()
{
    private IAggregateRepository<TAggregate> aggregateRepository;

    public CommandHandlerBase(IAggregateRepository<TAggregate> aggregateRepository)
    {
        this.aggregateRepository = aggregateRepository;
    }

    public void Handle(TCommand command)
    {
        var aggregate = aggregateRepository.FindById(command.Id);

        if (aggregate == null)
        {
            aggregate = new TAggregate() { Id = command.Id };

            aggregateRepository.Add(aggregate);
        }

        ProcessCommand(command, aggregate);

        var events = aggregate.GetUncommittedEvents();
        aggregateRepository.AddEvents(aggregate.Id, events);
        aggregate.MarkEventsAsCommitted();
    }

    protected abstract void ProcessCommand(TCommand command, TAggregate aggregate);
}