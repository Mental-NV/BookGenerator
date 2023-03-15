using BookGenerator.Application.Abstractions.Messsaging;
using BookGenerator.Application.Contracts.Books;
using BookGenerator.Domain.Core;
using BookGenerator.Domain.Primitives;
using BookGenerator.Domain.Primitives.Result;
using BookGenerator.Domain.Services;

namespace BookGenerator.Application.Books.Queries.GetStatus;

internal sealed class GetStatusByIdQueryHandler
    : IQueryHandler<GetStatusByIdQuery, GetStatusResponse>
{
    private readonly IBookRepository bookRepository;

    public GetStatusByIdQueryHandler(IBookRepository bookRepository)
    {
        this.bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
    }

    public async Task<Result<GetStatusResponse>> Handle(
        GetStatusByIdQuery request,
        CancellationToken cancellationToken)
    {
        Book book = await bookRepository.GetAsync(request.BookId);
        if (book == null)
        {
            return Result.Failure<GetStatusResponse>(new Error("Book.NotFound", $"Book with id {request.BookId} has not found"));
        }

        BookOrder result = new BookOrder()
        {
            BookId = book.Id,
            Title = book.Title,
            Status = book.Status
        };

        var response = new GetStatusResponse(result.Status, result.Title);
        return response;
    }
}
