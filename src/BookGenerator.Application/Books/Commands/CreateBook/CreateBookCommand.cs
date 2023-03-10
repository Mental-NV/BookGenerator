using BookGenerator.Application.Abstractions.Messsaging;
using BookGenerator.Application.Contracts.Books;

namespace BookGenerator.Application.Books.Commands.CreateBook;

public sealed record CreateBookCommand(
    string BookTitle) : ICommand<CreateBookResponse>;

