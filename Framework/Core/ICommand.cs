using MediatR;

namespace Framework.Core;

public interface ICommand : IRequest
{
    public Guid Id { get; }
}
