using BookGenerator.Domain.Services;
using OpenAI.Interfaces;
using OpenAI.Managers;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.ObjectModels;
using System.Text;
using BookGenerator.Domain.Core;
using Microsoft.Extensions.DependencyInjection;
using BookGenerator.Application.Abstractions.Data;
using BookGenerator.Domain.Repositories;
using OpenAI.ObjectModels.ResponseModels;

namespace BookGenerator.Infrastructure.Books;

public class BookCreaterChatGpt : IBookCreater
{
    private readonly IOpenAIService openAIService;
    private readonly IBookRepository bookRepository;
    private readonly IProgressRepository progressRepository;
    private readonly IChapterRepository chapterRepository;
    private readonly IUnitOfWork unitOfWork;

    public BookCreaterChatGpt(IOpenAIService openAIService, IBookRepository bookRepository, IProgressRepository progressRepository, IChapterRepository chapterRepository, IUnitOfWork unitOfWork)
    {
        this.openAIService = openAIService ?? throw new ArgumentNullException(nameof(openAIService));
        this.bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
        this.progressRepository = progressRepository ?? throw new ArgumentNullException(nameof(progressRepository));
        this.chapterRepository = chapterRepository ?? throw new ArgumentNullException(nameof(chapterRepository));
        this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task CreateAsync(Guid bookId)
    {
        Book book = await bookRepository.GetByIdAsync(bookId);
        BookProgress progress = await progressRepository.GetByIdAsync(bookId);
        string bookTitle = book.Title;

        progress.Progress = 5;
        progressRepository.Update(progress);
        await unitOfWork.SaveChangesAsync();

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
            progressRepository.Update(progress);
            await unitOfWork.SaveChangesAsync();
            return;
        }

        if (!tocResponse.Choices.Any())
        {
            progress.Status = BookStatus.Failed;
            progress.ErrorMessage = $"No response from ChatGPT API. {tocResponse}";
            progressRepository.Update(progress);
            await unitOfWork.SaveChangesAsync();
            return;
        }

        string tableOfContent = tocResponse.Choices.First().Text;
        progress.Progress = 10;
        progressRepository.Update(progress);
        await unitOfWork.SaveChangesAsync();

        IEnumerable<string> chapters = tableOfContent.Split("\n")
            .Where(chapter => chapter.Contains("Chapter ", StringComparison.InvariantCultureIgnoreCase));
        int chapterNumber = 1;
        foreach (string chapter in chapters)
        {
            int progressValue = 10 + (int)((85.0 * chapterNumber++) / chapters.Count());
            progress.Progress = progressValue;
            progressRepository.Update(progress);
            await unitOfWork.SaveChangesAsync();

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
                progressRepository.Update(progress);
                await unitOfWork.SaveChangesAsync();
                return;
            }

            if (!chapterResponse.Choices.Any())
            {
                progress.Status = BookStatus.Failed;
                progress.ErrorMessage = $"No response from ChatGPT API. {chapterResponse}";
                progressRepository.Update(progress);
                await unitOfWork.SaveChangesAsync();
                return;
            }

            string text = chapterResponse.Choices.First().Text;
            var chapterEntity = new Chapter() { Content = text, Title = chapter, Order = chapterNumber, BookId = book.Id };
            chapterRepository.Insert(chapterEntity);
            await unitOfWork.SaveChangesAsync(); 
        }

        progress.Progress = 100;
        progress.Status = BookStatus.Completed;
        progressRepository.Update(progress);
        await unitOfWork.SaveChangesAsync();
    }
}
