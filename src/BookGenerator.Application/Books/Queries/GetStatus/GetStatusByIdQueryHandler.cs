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
        BookProgress result = await bookRepository.GetProgressAsync(request.BookId);
        if (result == null)
        {
            return Result.Failure<GetStatusResponse>(new Error("BookProgress.NotFound", $"Book progress with id {request.BookId} has not found"));
        }

        var response = new GetStatusResponse(result.Title, result.Status, result.Progress);
        return response;
    }
}
