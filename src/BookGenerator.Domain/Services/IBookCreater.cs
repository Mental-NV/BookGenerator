using BookGenerator.Domain.Core;

namespace BookGenerator.Domain.Services;

public interface IBookCreater
{
    Task<Guid> CreateAsync(string bookTitle);
    Task<BookCreatingStatus> GetStatusAsync(Guid bookId);
    Task<Book> GetResultAsync(Guid bookId);
}
