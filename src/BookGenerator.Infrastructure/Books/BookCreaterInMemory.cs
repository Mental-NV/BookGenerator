﻿using BookGenerator.Application.Abstractions.Data;
using BookGenerator.Domain.Core;
using BookGenerator.Domain.Repositories;
using BookGenerator.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;

namespace BookGenerator.Infrastructure.Books;

public class BookCreaterInMemory : IBookCreater
{
    private readonly IBookRepository bookRepository;
    private readonly IUnitOfWork unitOfWork;
    private readonly IProgressRepository progressRepository;
    private readonly IServiceScopeFactory scopeFactory;

    public BookCreaterInMemory(IBookRepository bookRepository, IUnitOfWork unitOfWork, IProgressRepository progressRepository, IServiceScopeFactory scopeFactory)
    {
        this.bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
        this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this.progressRepository = progressRepository ?? throw new ArgumentNullException(nameof(progressRepository));
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
        progressRepository.Insert(progress);
        int rowAffected = await unitOfWork.SaveChangesAsync();
        if (rowAffected == 0)
        {
            throw new ApplicationException("Failed to create book");
        }

        var task = new Task(async () =>
        {
            await Task.Delay(3000);
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
                await scopedUnityOfWork.SaveChangesAsync();

                BookProgress scopedBookProgress = await scopedProgressRepository.GetByIdAsync(book.Id);
                scopedBookProgress.Progress = 100;
                scopedBookProgress.Status = BookStatus.Completed;
                scopedProgressRepository.Update(scopedBookProgress);
                await scopedUnityOfWork.SaveChangesAsync();

                scopedBookProgress = await scopedProgressRepository.GetByIdAsync(book.Id);
                Trace.WriteLine($"Book progress is {scopedBookProgress.Progress}");
            }

        });
        task.Start();
        await Task.CompletedTask;
    }
}
