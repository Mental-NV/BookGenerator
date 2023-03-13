using BookGenerator.Domain.Core;

namespace BookGenerator.Domain.Services;

public interface IBookCreater
{
    Task<Guid> CreateAsync(string bookTitle);
    Task<BookStatus> GetStatusAsync(Guid bookId);
    Task<Book> GetResultAsync(Guid bookId);
}
