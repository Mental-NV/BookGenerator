using BookGenerator.Application.Abstractions.Messsaging;
using BookGenerator.Domain.Core;
using BookGenerator.Domain.Primitives;
using BookGenerator.Domain.Primitives.Result;
using BookGenerator.Domain.Services;

namespace BookGenerator.Application.Books.Queries.GetBook;

internal sealed class GetBookByIdQueryHandler : IQueryHandler<GetBookByIdQuery, Book>
{
    private readonly IBookCreater bookCreater;

    public GetBookByIdQueryHandler(IBookCreater bookCreater)
    {
        this.bookCreater = bookCreater ?? throw new ArgumentNullException(nameof(bookCreater));
    }

    public async Task<Result<Book>> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
    {
        Book result = await bookCreater.GetResultAsync(request.BookId);
        if (result == null)
        {
            return Result.Failure<Book>(new Error("Book.NotFound", $"The book with id {request.BookId} has not found"));
        }

        return result;
    }
}
