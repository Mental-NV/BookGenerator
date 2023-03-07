using BookGenerator.Domain.Core;

namespace BookGenerator.Domain.Services;

public interface IBookCreater
{
    Guid Create(string bookTitle);
    BookCreatingStatus GetStatus(Guid bookId);
    Book GetResult(Guid bookId);
}
