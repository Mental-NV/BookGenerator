#nullable enable

namespace BookGenerator.Application.Abstractions.LLM;

/// <summary>
/// Abstraction for chat completion services compatible with OpenAI API.
/// </summary>
public interface IChatCompletionService
{
    /// <summary>
    /// Creates a chat completion request and returns the response text.
    /// </summary>
    /// <param name="prompt">The prompt/message to send to the LLM</param>
    /// <param name="maxTokens">Maximum tokens in the response (default: 4000)</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The completion text, or null if the request failed</returns>
    Task<string?> GetCompletionAsync(
        string prompt,
        int maxTokens = 4000,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the error message from the last failed request, if any.
    /// </summary>
    string? LastErrorMessage { get; }
}
