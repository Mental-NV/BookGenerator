using BookGenerator.Application.Abstractions.Messsaging;
using BookGenerator.Application.Contracts.Books;

namespace BookGenerator.Application.Books.Queries.GetStatus;

public sealed record GetStatusByIdQuery(Guid BookId) : IQuery<GetStatusResponse>;
