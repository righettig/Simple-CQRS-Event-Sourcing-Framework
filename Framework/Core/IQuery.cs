using MediatR;

namespace Framework.Core;

public interface IQuery<TResponse> : IRequest<TResponse>
{
}