using BookGenerator.Domain.Core;

namespace BookGenerator.Application.Contracts.Books;

public sealed record GetStatusResponse(BookStatus Status, string BookTitle);