using BookGenerator.Domain.Services;
using OpenAI.GPT3.Interfaces;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels.RequestModels;
using OpenAI.GPT3.ObjectModels;
using System.Text;
using BookGenerator.Domain.Core;
using Microsoft.Extensions.DependencyInjection;
using BookGenerator.Application.Abstractions.Data;

namespace BookGenerator.Infrastructure.Books;

public class BookCreaterChatGpt : IBookCreater
{
    private readonly IOpenAIService openAIService;
    private readonly IBookRepository bookRepository;
    private readonly IUnitOfWork unitOfWork;
    private readonly IServiceScopeFactory scopeFactory;

    public BookCreaterChatGpt(IOpenAIService openAIService, IBookRepository bookRepository, IUnitOfWork unitOfWork, IServiceScopeFactory scopeFactory)
    {
        this.openAIService = openAIService ?? throw new ArgumentNullException(nameof(openAIService));
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
            BookId = book.Id,
            Progress = 5,
            Status = BookStatus.Pending,
            Title = bookTitle
        };
        await bookRepository.InsertProgressAsync(progress);
        int rowAffected = await unitOfWork.SaveChangesAsync();
        if (rowAffected == 0)
        {
            throw new ApplicationException("Failed to create book");
        }

        var task = new Task(async () =>
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var scopedBookRepository = scope.ServiceProvider.GetRequiredService<IBookRepository>();
                var scopedUnitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                while (true)
                {
                    var completionResult = await openAIService.Completions.CreateCompletion(
                        new CompletionCreateRequest()
                        {
                            Prompt = $"Write a table of content for a book with name '{bookTitle}'. Please format list the chapters in format 'Chapter #: Title'",
                            Model = Models.TextDavinciV3,
                            MaxTokens = 4000
                        }
                    );
                    if (completionResult.Successful && completionResult.Choices.Any())
                    {
                        string tableOfContent = completionResult.Choices.First().Text;
                        Console.WriteLine(tableOfContent);
                        progress.Progress = 10;
                        await scopedBookRepository.UpdateProgressAsync(progress);
                        await scopedUnitOfWork.SaveChangesAsync();
                        
                        IEnumerable<string> chapters = tableOfContent.Split("\n").Where(chapter => chapter.Contains("Chapter ", StringComparison.InvariantCultureIgnoreCase));
                        int i = 0;
                        foreach (string chapter in chapters)
                        {
                            i++;
                            int progressValue = 10 + (int)((85.0 * i) / chapters.Count());
                            progress.Progress = progressValue;
                            await scopedBookRepository.UpdateProgressAsync(progress);
                            await scopedUnitOfWork.SaveChangesAsync();
                            while (true)
                            {
                                var chapterCompletion = await openAIService.Completions.CreateCompletion(
                                    new CompletionCreateRequest()
                                    {
                                        Prompt = $"Write a chapter '{chapter} for the book '{bookTitle}'. Format titles with leading #. Format subtitles with leading ##.",
                                        Model = Models.TextDavinciV3,
                                        MaxTokens = 4000
                                    }
                                );
                                if (chapterCompletion.Successful && chapterCompletion.Choices.Any())
                                {
                                    string text = chapterCompletion.Choices.First().Text;
                                    var chapterEntity = new Chapter() { Content = text, Title = chapter, Order = i, BookId = book.Id };
                                    await scopedBookRepository.InsertChapterAsync(chapterEntity);
                                    await scopedUnitOfWork.SaveChangesAsync();
                                    break;
                                }
                                await Task.Delay(2000);
                            }
                        }
                        break;
                    }
                    await Task.Delay(2000);
                }
                progress.Progress = 100;
                progress.Status = BookStatus.Completed;
                await scopedBookRepository.UpdateProgressAsync(progress);
                await scopedUnitOfWork.SaveChangesAsync();
            }
        });
        task.Start();
        return await Task.FromResult(book.Id);
    }
}
