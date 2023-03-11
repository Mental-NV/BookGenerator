using BookGenerator.Application.Abstractions.Messsaging;
using BookGenerator.Domain.Core;

namespace BookGenerator.Application.Books.Queries.GetBook;

public record GetBookByIdQuery(Guid BookId) : IQuery<Book>;
