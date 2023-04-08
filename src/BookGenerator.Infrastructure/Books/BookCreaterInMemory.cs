using BookGenerator.Application.Abstractions.Data;
using BookGenerator.Domain.Core;
using BookGenerator.Domain.Repositories;
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

    public async Task CreateAsync(Guid boodId)
    {
        string bookTitle = $"Book {boodId}";
        Book book = new Book(bookTitle);
        BookProgress progress = new(book)
        {
            Progress = 5,
            Status = BookStatus.Pending,
            Title = bookTitle
        };
        bookRepository.Insert(book);
        int rowAffected = await unitOfWork.SaveChangesAsync();
        if (rowAffected == 0)
        {
            throw new ApplicationException("Failed to create book");
        }

        var task = new Task(async () =>
        {
            await Task.Delay(15000);
            using (var scope = scopeFactory.CreateScope())
            {
                var scopedChapterRepository = scope.ServiceProvider.GetRequiredService<IChapterRepository>();
                var scopedProgressRepository = scope.ServiceProvider.GetRequiredService<IProgressRepository>();
                var scopedUnityOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                for (int i = 0; i < 10; i++)
                {
                    scopedChapterRepository.Insert(new Chapter()
                    {
                        Order = i + 1,
                        Title = $"Chapter {i + 1}",
                        Content = $"Content {i + 1}",
                        BookId = book.Id
                    });
                }

                BookProgress scopedBookProgress = await scopedProgressRepository.GetByIdAsync(book.Id);
                scopedBookProgress.Progress = 100;
                scopedBookProgress.Status = BookStatus.Completed;
                scopedProgressRepository.Update(scopedBookProgress);
                await scopedUnityOfWork.SaveChangesAsync();
            }

        });
        task.Start();
        // return await Task.FromResult(book.Id);
        await Task.CompletedTask;
    }
}
