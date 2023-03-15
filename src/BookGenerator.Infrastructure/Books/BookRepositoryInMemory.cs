using BookGenerator.Domain.Core;
using BookGenerator.Domain.Services;
using System.Collections.Concurrent;

namespace BookGenerator.Infrastructure.Books;

public class BookRepositoryInMemory : IBookRepository
{
    private readonly static ConcurrentDictionary<Guid, Book> books = new ConcurrentDictionary<Guid, Book>();

    public async Task<Book> GetAsync(Guid bookId)
    {
        books.TryGetValue(bookId, out Book book);
        return await Task.FromResult(book);
    }

    public async Task SetAsync(Guid bookId, Book newBook)
    {
        books[bookId] = newBook;
        await Task.FromResult(newBook);
    }
}
