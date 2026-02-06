#nullable enable

using BookGenerator.Domain.Services;
using BookGenerator.Domain.Core;
using BookGenerator.Application.Abstractions.Data;
using BookGenerator.Application.Abstractions.LLM;
using BookGenerator.Domain.Repositories;

namespace BookGenerator.Infrastructure.Books;

public class BookCreaterChatGpt : IBookCreater
{
    private readonly IChatCompletionService chatCompletionService;
    private readonly IBookRepository bookRepository;
    private readonly IProgressRepository progressRepository;
    private readonly IChapterRepository chapterRepository;
    private readonly IUnitOfWork unitOfWork;

    public BookCreaterChatGpt(IChatCompletionService chatCompletionService, IBookRepository bookRepository, IProgressRepository progressRepository, IChapterRepository chapterRepository, IUnitOfWork unitOfWork)
    {
        this.chatCompletionService = chatCompletionService ?? throw new ArgumentNullException(nameof(chatCompletionService));
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

        // Generate Table of Contents
        string tocPrompt = $"Write a table of contents for a book with name '{bookTitle}'. Please format the chapters in the format 'Chapter #: Title' on each line.";
        string? tableOfContent = await chatCompletionService.GetCompletionAsync(tocPrompt, maxTokens: 4000);

        if (string.IsNullOrEmpty(tableOfContent))
        {
            progress.Status = BookStatus.Failed;
            progress.ErrorMessage = chatCompletionService.LastErrorMessage ?? "Failed to generate table of contents";
            progressRepository.Update(progress);
            await unitOfWork.SaveChangesAsync();
            return;
        }

        progress.Progress = 10;
        progressRepository.Update(progress);
        await unitOfWork.SaveChangesAsync();

        // Parse chapters from table of contents
        IEnumerable<string> chapters = tableOfContent.Split("\n")
            .Where(chapter => !string.IsNullOrWhiteSpace(chapter) && chapter.Contains("Chapter ", StringComparison.InvariantCultureIgnoreCase))
            .ToList();

        if (!chapters.Any())
        {
            progress.Status = BookStatus.Failed;
            progress.ErrorMessage = "No chapters found in generated table of contents";
            progressRepository.Update(progress);
            await unitOfWork.SaveChangesAsync();
            return;
        }

        int totalChapters = chapters.Count();
        int chapterNumber = 1;

        // Generate content for each chapter
        foreach (string chapter in chapters)
        {
            int progressValue = 10 + (int)((85.0 * chapterNumber) / totalChapters);
            progress.Progress = progressValue;
            progressRepository.Update(progress);
            await unitOfWork.SaveChangesAsync();

            string chapterPrompt = $"Write a detailed chapter titled '{chapter.Trim()}' for the book '{bookTitle}'. " +
                $"Format the main title with a leading #. Format subtitles with leading ##. " +
                $"Write comprehensive content for this chapter.";

            string? chapterContent = await chatCompletionService.GetCompletionAsync(chapterPrompt, maxTokens: 4000);

            if (string.IsNullOrEmpty(chapterContent))
            {
                progress.Status = BookStatus.Failed;
                progress.ErrorMessage = chatCompletionService.LastErrorMessage ?? $"Failed to generate content for {chapter}";
                progressRepository.Update(progress);
                await unitOfWork.SaveChangesAsync();
                return;
            }

            var chapterEntity = new Chapter
            {
                Content = chapterContent,
                Title = chapter.Trim(),
                Order = chapterNumber,
                BookId = book.Id
            };
            chapterRepository.Insert(chapterEntity);
            await unitOfWork.SaveChangesAsync();

            chapterNumber++;
        }

        progress.Progress = 100;
        progress.Status = BookStatus.Completed;
        progressRepository.Update(progress);
        await unitOfWork.SaveChangesAsync();
    }
}
