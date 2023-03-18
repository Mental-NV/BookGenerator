using BookGenerator.Domain.Core;
using BookGenerator.Domain.Services;
using System.Collections.Concurrent;

namespace BookGenerator.Infrastructure.Books;

public class BookRepositoryInMemory : IBookRepository
{
    private readonly static ConcurrentDictionary<Guid, Book> bookStore = new ConcurrentDictionary<Guid, Book>();
    private readonly static ConcurrentDictionary<Guid, BookProgress> progressStore = new ConcurrentDictionary<Guid, BookProgress>();

    public async Task<Book> GetAsync(Guid bookId)
    {
        bookStore.TryGetValue(bookId, out Book book);
        return await Task.FromResult(book);
    }

    public async Task SetAsync(Guid bookId, Book newBook)
    {
        bookStore[bookId] = newBook;
        await Task.FromResult(newBook);
    }

    public async Task<BookProgress> GetProgressAsync(Guid bookId)
    {
        progressStore.TryGetValue(bookId, out BookProgress progress);
        return await Task.FromResult(progress);
    }

    public async Task SetProgressAsync(Guid bookId, BookProgress bookProgress)
    {
        progressStore[bookId] = bookProgress;
        await Task.FromResult(bookProgress);
    }
}
