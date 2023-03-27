using BookGenerator.Application.Abstractions.Data;
using BookGenerator.Domain.Core;
using BookGenerator.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace BookGenerator.Infrastructure.Books;

public class BookCreaterInMemory : IBookCreater
{
    private readonly IBookRepository bookRepository;
    private readonly IUnitOfWork unitOfWork;
    private readonly IServiceScopeFactory scopeFactory;

    public BookCreaterInMemory(IBookRepository bookRepository, IUnitOfWork unitOfWork, IServiceScopeFactory scopeFactory)
    {
        this.bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
        this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this.scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
    }

    public async Task<Guid> CreateAsync(string bookTitle)
    {
        Book book = new Book(bookTitle);
        await bookRepository.InsertAsync(book);
        BookProgress progress = new BookProgress()
        {
            Progress = 5,
            BookId = book.Id,
            Status = BookStatus.Pending,
            Title = bookTitle
        };
        await bookRepository.InsertProgressAsync(progress);
        await unitOfWork.SaveChangesAsync();
        var task = new Task(async () =>
        {
            await Task.Delay(15000);
            using (var scope = scopeFactory.CreateScope())
            {
                var scopedBookRepository = scope.ServiceProvider.GetRequiredService<IBookRepository>();
                var scopedUnityOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                for (int i = 0; i < 10; i++)
                {
                    await scopedBookRepository.InsertChapterAsync(new Chapter()
                    {
                        Order = i + 1,
                        Title = $"Chapter {i + 1}",
                        Content = $"Content {i + 1}",
                        BookId = book.Id
                    });
                }

                BookProgress scopedBookProgress = await scopedBookRepository.GetProgressAsync(book.Id);
                scopedBookProgress.Progress = 100;
                scopedBookProgress.Status = BookStatus.Completed;
                await scopedBookRepository.UpdateProgressAsync(scopedBookProgress);
                await scopedUnityOfWork.SaveChangesAsync();
            }

        });
        task.Start();
        return await Task.FromResult(book.Id);
    }
}
