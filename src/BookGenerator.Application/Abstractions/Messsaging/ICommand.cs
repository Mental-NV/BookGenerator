using BookGenerator.Domain.Primitives.Result;
using MediatR;

namespace BookGenerator.Application.Abstractions.Messsaging;

public interface ICommand : IRequest<Result>
{
}

public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}