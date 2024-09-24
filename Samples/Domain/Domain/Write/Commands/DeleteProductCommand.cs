using Framework.Core;

namespace Domain.Write.Commands;

public class DeleteProductCommand(Guid id) : ICommand
{
    public Guid Id { get; } = id;
}