using BookGenerator.Domain.Core;

namespace BookGenerator.Domain.Services;

public interface IBookRepository
{
    Task<Book> GetAsync(Guid bookId);
    Task SetAsync(Guid bookId, Book newBook);
}
