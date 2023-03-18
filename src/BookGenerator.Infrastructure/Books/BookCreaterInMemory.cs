using BookGenerator.Domain.Core;
using BookGenerator.Domain.Services;
using System.Collections.Concurrent;

namespace BookGenerator.Infrastructure.Books;

public class BookCreaterInMemory : IBookCreater
{
    private readonly IBookRepository bookRepository;

    public BookCreaterInMemory(IBookRepository bookRepository)
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
        };
        await bookRepository.SetAsync(id, book);
        BookProgress progress = new BookProgress()
        {
            Progress = 5,
            BookId = id,
            Status = BookStatus.Pending,
            Title = bookTitle
        };
        await bookRepository.SetProgressAsync(id, progress);
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
            await bookRepository.SetAsync(id, book);
            progress.Progress = 100;
            progress.Status = BookStatus.Completed;
            await bookRepository.SetProgressAsync(id, progress);
        });
        task.Start();
        return await Task.FromResult(id);
    }
}
