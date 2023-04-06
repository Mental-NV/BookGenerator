using BookGenerator.Domain.Services;
using OpenAI.GPT3.Interfaces;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels.RequestModels;
using OpenAI.GPT3.ObjectModels;
using System.Text;
using BookGenerator.Domain.Core;
using Microsoft.Extensions.DependencyInjection;
using BookGenerator.Application.Abstractions.Data;
using BookGenerator.Domain.Repositories;
using OpenAI.GPT3.ObjectModels.ResponseModels;

namespace BookGenerator.Infrastructure.Books;

public class BookCreaterChatGpt : IBookCreater
{
    private readonly IOpenAIService openAIService;
    private readonly IBookRepository bookRepository;
    private readonly IProgressRepository progressRepository;
    private readonly IUnitOfWork unitOfWork;
    private readonly IServiceScopeFactory scopeFactory;

    public BookCreaterChatGpt(IOpenAIService openAIService, IBookRepository bookRepository, IProgressRepository progressRepository, IUnitOfWork unitOfWork, IServiceScopeFactory scopeFactory)
    {
        this.openAIService = openAIService ?? throw new ArgumentNullException(nameof(openAIService));
        this.bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
        this.progressRepository = progressRepository ?? throw new ArgumentNullException(nameof(progressRepository));
        this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this.scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
    }

    public async Task<Guid> CreateAsync(string bookTitle)
    {
        Book book = new Book(bookTitle);
        BookProgress progress = new BookProgress(book)
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
            using (var scope = scopeFactory.CreateScope())
            {
                var scopedProgressRepository = scope.ServiceProvider.GetRequiredService<IProgressRepository>();
                var scopedChapterRepository = scope.ServiceProvider.GetRequiredService<IChapterRepository>();
                var scopedUnitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                CompletionCreateResponse tocResponse = null;
                int tocAttempt = 0;
                const int maxAttempts = 3;
                while (tocAttempt++ < maxAttempts)
                {
                    tocResponse = await openAIService.Completions.CreateCompletion(
                        new CompletionCreateRequest()
                        {
                            Prompt = $"Write a table of content for a book with name '{bookTitle}'. Please format list the chapters in format 'Chapter #: Title'",
                            Model = Models.TextDavinciV3,
                            MaxTokens = 4000
                        }
                    );

                    if (tocResponse.Successful)
                    {
                        break;
                    }
                    await Task.Delay(2000);
                }

                if (!tocResponse.Successful)
                {
                    progress.Status = BookStatus.Failed;
                    progress.ErrorMessage = tocResponse.Error.ToString();
                    scopedProgressRepository.Update(progress);
                    await scopedUnitOfWork.SaveChangesAsync();
                    return;
                }

                if (!tocResponse.Choices.Any())
                {
                    progress.Status = BookStatus.Failed;
                    progress.ErrorMessage = $"No response from ChatGPT API. {tocResponse}";
                    scopedProgressRepository.Update(progress);
                    await scopedUnitOfWork.SaveChangesAsync();
                    return;
                }

                string tableOfContent = tocResponse.Choices.First().Text;
                progress.Progress = 10;
                scopedProgressRepository.Update(progress);
                await scopedUnitOfWork.SaveChangesAsync();

                IEnumerable<string> chapters = tableOfContent.Split("\n")
                    .Where(chapter => chapter.Contains("Chapter ", StringComparison.InvariantCultureIgnoreCase));
                int chapterNumber = 1;
                foreach (string chapter in chapters)
                {
                    int progressValue = 10 + (int)((85.0 * chapterNumber++) / chapters.Count());
                    progress.Progress = progressValue;
                    scopedProgressRepository.Update(progress);
                    await scopedUnitOfWork.SaveChangesAsync();

                    CompletionCreateResponse chapterResponse = null;
                    int chapterAttempt = 0;
                    while (chapterAttempt++ < maxAttempts)
                    {
                        chapterResponse = await openAIService.Completions.CreateCompletion(
                            new CompletionCreateRequest()
                            {
                                Prompt = $"Write a chapter '{chapter} for the book '{bookTitle}'. Format titles with leading #. Format subtitles with leading ##.",
                                Model = Models.TextDavinciV3,
                                MaxTokens = 4000
                            }
                        );

                        if (chapterResponse.Successful)
                        {
                            break;
                        }
                        await Task.Delay(2000);
                    }

                    if (!chapterResponse.Successful)
                    {
                        progress.Status = BookStatus.Failed;
                        progress.ErrorMessage = chapterResponse.Error.ToString();
                        scopedProgressRepository.Update(progress);
                        await scopedUnitOfWork.SaveChangesAsync();
                        return;
                    }

                    if (!chapterResponse.Choices.Any())
                    {
                        progress.Status = BookStatus.Failed;
                        progress.ErrorMessage = $"No response from ChatGPT API. {chapterResponse}";
                        scopedProgressRepository.Update(progress);
                        await scopedUnitOfWork.SaveChangesAsync();
                        return;
                    }

                    string text = chapterResponse.Choices.First().Text;
                    var chapterEntity = new Chapter() { Content = text, Title = chapter, Order = chapterNumber, BookId = book.Id };
                    scopedChapterRepository.Insert(chapterEntity);
                    await scopedUnitOfWork.SaveChangesAsync(); 
                }

                progress.Progress = 100;
                progress.Status = BookStatus.Completed;
                scopedProgressRepository.Update(progress);
                await scopedUnitOfWork.SaveChangesAsync();
            }
        });
        task.Start();
        return await Task.FromResult(book.Id);
    }
}
