using BookGenerator.Domain.Core;
using BookGenerator.Domain.Services;
using System.Collections.Concurrent;

namespace BookGenerator.Infrastructure.Books;

public class BookCreater : IBookCreater
{
    private readonly static Book pendingBook = new Book();
    private readonly static ConcurrentDictionary<Guid, Book> books = new ConcurrentDictionary<Guid, Book>();

    public async Task<Guid> CreateAsync(string bookTitle)
    {
        Guid id = Guid.NewGuid();
        books[id] = pendingBook;
        var task = new Task(async () =>
        {
            await Task.Delay(60000);
            Book book = new Book()
            {
                Title = bookTitle,
                Id = id
            };
            for (int i = 0; i < 10; i++)
            {
                book.Chapters.Add(new Chapter()
                {
                    Title = $"Chapter {i + 1}",
                    Content = $"Content {i + 1}"
                });
            }
            books[id] = book;
        });
        task.Start();
        return await Task.FromResult(id);
    }

    public async Task<Book> GetResultAsync(Guid bookId)
    {
        books.TryGetValue(bookId, out Book book);
        return await Task.FromResult(book);
    }

    public async Task<BookCreatingStatus> GetStatusAsync(Guid bookId)
    {
        if (books.TryGetValue(bookId, out Book book) && book != null)
        {
            if (book == pendingBook)
            {
                return await Task.FromResult(BookCreatingStatus.Pedning);
            }
            
            if (!string.IsNullOrEmpty(book.Title) && book.Chapters.Count > 0)
            {
                return await Task.FromResult(BookCreatingStatus.Completed);
            }

            return await Task.FromResult(BookCreatingStatus.Failed);
        }
        
        return await Task.FromResult(BookCreatingStatus.None);
    }
}
