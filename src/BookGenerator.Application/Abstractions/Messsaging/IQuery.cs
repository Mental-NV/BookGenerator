using BookGenerator.Domain.Primitives.Result;
using MediatR;

namespace BookGenerator.Application.Abstractions.Messsaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}
