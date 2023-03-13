using BookGenerator.Domain.Core;

namespace BookGenerator.Application.Contracts.Books;

public sealed record GetStatusResponse(BookCreatingStatus Status, string BookTitle);