using BookGenerator.Application.Books.Commands.CreateBook;
using BookGenerator.Application.Books.Queries.GetBook;
using BookGenerator.Application.Books.Queries.GetStatus;
using BookGenerator.Application.Contracts.Books;
using BookGenerator.Domain.Core;
using BookGenerator.Domain.DomainEvents;
using BookGenerator.Domain.Primitives.Result;
using BookGenerator.Domain.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookGenerator.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookController(ISender sender, IBookConverter converter, IPublisher publisher) : ApiController(sender)
{
    private readonly IBookConverter converter = converter ?? throw new ArgumentNullException(nameof(converter));
    private readonly IPublisher publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] string title, CancellationToken cancellationToken)
    {
        var command = new CreateBookCommand(title);
        Result<CreateBookResponse> result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        await publisher.Publish(new BookCreationStartedDomainEvent(result.Value.BookId));

        return Ok(result.Value);
    }

    [HttpGet("{bookId}")]
    public async Task<IActionResult> GetStatus(Guid bookId, CancellationToken cancellationToken)
    {
        GetStatusByIdQuery query = new GetStatusByIdQuery(bookId);
        Result<GetStatusResponse> result = await sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return Ok(result.Value);
    }

    [HttpGet("download/{bookId}")]
    public async Task<IActionResult> Download(Guid bookId, CancellationToken cancellationToken)
    {
        GetBookByIdQuery query = new GetBookByIdQuery(bookId);
        var result = await sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        BookFile file = converter.Convert(result.Value);

        return Ok(file);
    }

    [HttpGet("download2/{bookId}")]
    public async Task<IActionResult> Download2(Guid bookId, CancellationToken cancellationToken)
    {
        GetBookByIdQuery query = new GetBookByIdQuery(bookId);
        var result = await sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        BookFile bookFile = converter.Convert(result.Value);

        MemoryStream contentStream = new MemoryStream(bookFile.Content);

        return new FileStreamResult(contentStream, bookFile.ContentType)
        {
            FileDownloadName = bookFile.Name
        };
    }
}
