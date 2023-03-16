using BookGenerator.Domain.Services;
using OpenAI.GPT3.Interfaces;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels.RequestModels;
using OpenAI.GPT3.ObjectModels;
using System.Text;
using BookGenerator.Domain.Core;

namespace BookGenerator.Infrastructure.Books;

public class BookCreaterChatGpt : IBookCreater
{
    private readonly IOpenAIService openAIService;
    private readonly IBookRepository bookRepository;

    public BookCreaterChatGpt(IOpenAIService openAIService, IBookRepository bookRepository)
    {
        this.openAIService = openAIService ?? throw new ArgumentNullException(nameof(openAIService));
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

                    IEnumerable<string> chapters = tableOfContent.Split("\n").Where(chapter => chapter.Contains("Chapter ", StringComparison.InvariantCultureIgnoreCase));
                    int i = 0;
                    foreach (string chapter in chapters)
                    {
                        i++;
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
                                book.Chapters.Add(new Chapter() { Content = text, Title = chapter, Id = i });
                                break;
                            }
                        }
                    }
                    break;
                }
            }
            book.Status = BookStatus.Completed;
            await bookRepository.SetAsync(id, book);
        });
        task.Start();
        return await Task.FromResult(id);
    }
}
