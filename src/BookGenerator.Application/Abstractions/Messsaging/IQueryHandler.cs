using BookGenerator.Domain.Primitives.Result;
using MediatR;

namespace BookGenerator.Application.Abstractions.Messsaging;

public interface IQueryHandler<TQuery, TResponse>
    : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{
}
