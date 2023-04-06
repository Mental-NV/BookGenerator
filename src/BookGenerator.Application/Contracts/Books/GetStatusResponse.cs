using BookGenerator.Domain.Core;

namespace BookGenerator.Application.Contracts.Books;

public sealed record GetStatusResponse(string BookTitle, BookStatus Status, int Progress, string ErrorMessage);