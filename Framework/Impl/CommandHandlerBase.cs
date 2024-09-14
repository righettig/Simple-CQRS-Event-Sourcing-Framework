using Framework.Core;
using MediatR;

namespace Framework.Impl;

public abstract class CommandHandlerBase<TCommand, TAggregate> : IRequestHandler<TCommand>
    where TCommand : ICommand
    where TAggregate : IAggregateRoot, new()
{
    private readonly IAggregateRepository<TAggregate> aggregateRepository;

    public CommandHandlerBase(IAggregateRepository<TAggregate> aggregateRepository)
    {
        this.aggregateRepository = aggregateRepository;
    }

    public Task Handle(TCommand command, CancellationToken cancellationToken)
    {
        var aggregate = aggregateRepository.GetById(command.Id);

        ProcessCommand(command, aggregate);

        aggregateRepository.Save(aggregate);

        return Task.CompletedTask;
    }

    protected abstract void ProcessCommand(TCommand command, TAggregate aggregate);
}