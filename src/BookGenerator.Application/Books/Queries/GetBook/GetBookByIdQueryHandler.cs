using BookGenerator.Application.Abstractions.Messsaging;
using BookGenerator.Domain.Core;
using BookGenerator.Domain.Primitives;
using BookGenerator.Domain.Primitives.Result;
using BookGenerator.Domain.Repositories;

namespace BookGenerator.Application.Books.Queries.GetBook;

internal sealed class GetBookByIdQueryHandler : IQueryHandler<GetBookByIdQuery, Book>
{
    private readonly IBookRepository bookRepository;

    public GetBookByIdQueryHandler(IBookRepository bookRepository)
    {
        this.bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
    }

    public async Task<Result<Book>> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
    {
        Book result = await bookRepository.GetByIdAsync(request.BookId);
        if (result == null)
        {
            return Result.Failure<Book>(new Error("Book.NotFound", $"The book with id {request.BookId} has not found"));
        }

        return result;
    }
}
