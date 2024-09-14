namespace Domain.Write.Commands.Handlers;

public interface ICommandHandler<TCommand>
{
    public void Handle(TCommand command);
}