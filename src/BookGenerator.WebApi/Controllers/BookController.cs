using BookGenerator.Application.Books.Commands.CreateBook;
using BookGenerator.Application.Books.Queries.GetBook;
using BookGenerator.Application.Books.Queries.GetStatus;
using BookGenerator.Application.Contracts.Books;
using BookGenerator.Domain.Core;
using BookGenerator.Domain.Primitives.Result;
using BookGenerator.Domain.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookGenerator.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookController : ControllerBase
{
    private readonly ISender sender;
    private readonly IBookConverter converter;

    public BookController(ISender sender, IBookConverter converter)
    {
        this.sender = sender ?? throw new ArgumentNullException(nameof(sender));
        this.converter = converter ?? throw new ArgumentNullException(nameof(converter));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] string title, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException($"'{nameof(title)}' cannot be null or whitespace.", nameof(title));
        }

        var command = new CreateBookCommand(title);
        Result<CreateBookResponse> result = await sender.Send(command, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{bookId}")]
    public async Task<IActionResult> GetStatus(Guid bookId, CancellationToken cancellationToken)
    {
        GetStatusByIdQuery query = new GetStatusByIdQuery(bookId);
        Result<GetStatusResponse> result = await sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("download/{bookId}")]
    public async Task<IActionResult> Download(Guid bookId, CancellationToken cancellationToken)
    {
        GetBookByIdQuery query = new GetBookByIdQuery(bookId);
        var result = await sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        BookFile file = converter.ToTextFile(result.Value);

        // return File(file.Content, file.ContentType, file.Name, false);
        return Ok(file);
    }
}
