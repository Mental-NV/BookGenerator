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
    private readonly IBookCreater bookCreater;

    public GetStatusByIdQueryHandler(IBookCreater bookCreater)
    {
        this.bookCreater = bookCreater ?? throw new ArgumentNullException(nameof(bookCreater));
    }

    public async Task<Result<GetStatusResponse>> Handle(
        GetStatusByIdQuery request,
        CancellationToken cancellationToken)
    {
        BookStatus result = await bookCreater.GetStatusAsync(request.BookId);

        if (result == null)
        {
            return Result.Failure<GetStatusResponse>(new Error("BookStatus.NotFound", $"Book status with id {request.BookId} has not found"));
        }

        var response = new GetStatusResponse(result.Status, result.Title);
        return response;
    }
}
