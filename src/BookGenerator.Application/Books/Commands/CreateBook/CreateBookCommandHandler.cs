using BookGenerator.Application.Abstractions.Messsaging;
using BookGenerator.Application.Contracts.Books;
using BookGenerator.Domain.Primitives.Result;
using BookGenerator.Domain.Services;

namespace BookGenerator.Application.Books.Commands.CreateBook;

internal sealed class CreateBookCommandHandler : ICommandHandler<CreateBookCommand, CreateBookResponse>
{
    private readonly IBookCreater bookCreater;

    public CreateBookCommandHandler(IBookCreater bookCreater)
    {
        this.bookCreater = bookCreater ?? throw new ArgumentNullException(nameof(bookCreater));
    }

    public async Task<Result<CreateBookResponse>> Handle(CreateBookCommand request, CancellationToken cancellationToken)
    {
        Guid id = await bookCreater.CreateAsync(request.BookTitle);
        return new CreateBookResponse() {  Id = id };
    }
}
