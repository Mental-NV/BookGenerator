using BookGenerator.Domain.Core;
using BookGenerator.Domain.Services;
using System.Collections.Concurrent;

namespace BookGenerator.Infrastructure.Books;

public class BookCreater : IBookCreater
{
    private readonly IBookRepository bookRepository;

    public BookCreater(IBookRepository bookRepository)
    {
        this.bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
    }

    public async Task<Guid> CreateAsync(string bookTitle)
    {
        Guid id = Guid.NewGuid();
        Book book = new Book()
        {
            Title = bookTitle,
            Id = id,
            Status = BookStatus.Pending
        };
        await bookRepository.SetAsync(id, book);
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
            book.Status = BookStatus.Completed;
            await bookRepository.SetAsync(id, book);
        });
        task.Start();
        return await Task.FromResult(id);
    }
}
