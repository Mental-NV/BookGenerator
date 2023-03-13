using BookGenerator.Domain.Core;
using BookGenerator.Domain.Services;
using System.Collections.Concurrent;

namespace BookGenerator.Infrastructure.Books;

public class BookCreater : IBookCreater
{
    private readonly static ConcurrentDictionary<Guid, Book> books = new ConcurrentDictionary<Guid, Book>();

    public async Task<Guid> CreateAsync(string bookTitle)
    {
        Guid id = Guid.NewGuid();
        Book book = new Book()
        {
            Title = bookTitle,
            Id = id,
            Status = BookCreatingStatus.Pending
        };
        books[id] = book;
        var task = new Task(async () =>
        {
            await Task.Delay(15000);

            for (int i = 0; i < 10; i++)
            {
                book.Chapters.Add(new Chapter()
                {
                    Title = $"Chapter {i + 1}",
                    Content = $"Content {i + 1}"
                });
            }
            book.Status = BookCreatingStatus.Completed;
        });
        task.Start();
        return await Task.FromResult(id);
    }

    public async Task<Book> GetResultAsync(Guid bookId)
    {
        books.TryGetValue(bookId, out Book book);
        return await Task.FromResult(book);
    }

    public async Task<BookStatus> GetStatusAsync(Guid bookId)
    {
        if (books.TryGetValue(bookId, out Book book))
        {
            return await Task.FromResult(new BookStatus() { Status = book.Status, Title = book.Title });
        }
        else
        {
            return null;
        }
    }
}
