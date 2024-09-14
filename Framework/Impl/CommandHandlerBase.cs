using Framework.Core;
using MediatR;

namespace Framework.Impl;

public abstract class CommandHandlerBase<TCommand, TAggregate> : IRequestHandler<TCommand>
    where TCommand : ICommand
    where TAggregate : IAggregateRoot, new()
{
    private IAggregateRepository<TAggregate> aggregateRepository;

    public CommandHandlerBase(IAggregateRepository<TAggregate> aggregateRepository)
    {
        this.aggregateRepository = aggregateRepository;
    }

    public Task Handle(TCommand command, CancellationToken cancellationToken)
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

        return Task.CompletedTask;
    }

    protected abstract void ProcessCommand(TCommand command, TAggregate aggregate);
}