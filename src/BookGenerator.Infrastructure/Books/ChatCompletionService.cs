#nullable enable

using OpenAI.Chat;

namespace BookGenerator.Infrastructure.Books;

/// <summary>
/// Implementation of chat completion service using the official OpenAI SDK.
/// </summary>
public class ChatCompletionService : Application.Abstractions.LLM.IChatCompletionService
{
    private readonly ChatClient chatClient;
    private string? lastErrorMessage;

    public string? LastErrorMessage => lastErrorMessage;

    public ChatCompletionService(ChatClient chatClient)
    {
        this.chatClient = chatClient ?? throw new ArgumentNullException(nameof(chatClient));
    }

    public async Task<string?> GetCompletionAsync(string prompt, int maxTokens = 4000, CancellationToken cancellationToken = default)
    {
        try
        {
            lastErrorMessage = null;

            var messages = new List<ChatMessage>
            {
                new UserChatMessage(prompt)
            };

            var options = new ChatCompletionOptions
            {
                MaxOutputTokenCount = maxTokens,
                Temperature = 0.7f
            };

            ChatCompletion completion = await chatClient.CompleteChatAsync(messages, options, cancellationToken);

            if (completion?.Content == null || completion.Content.Count == 0)
            {
                lastErrorMessage = "No response content received from OpenAI API";
                return null;
            }

            return completion.Content[0].Text;
        }
        catch (Exception ex)
        {
            lastErrorMessage = $"Error calling OpenAI API: {ex.Message}";
            return null;
        }
    }
}
