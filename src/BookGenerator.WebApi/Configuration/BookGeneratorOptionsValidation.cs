using Microsoft.Extensions.Options;

namespace BookGenerator.WebApi.Configuration;

public class BookGeneratorOptionsValidation : IValidateOptions<BookGeneratorOptions>
{
    public ValidateOptionsResult Validate(string name, BookGeneratorOptions options)
    {
        if (string.IsNullOrEmpty(options.OpenAIApiKey))
        {
            return ValidateOptionsResult.Fail("OpenAI API key is required.");
        }

        if (string.IsNullOrEmpty(options.OpenAIOrganization))
        {
            return ValidateOptionsResult.Fail("OpenAI organization is required.");
        }

        if (string.IsNullOrEmpty(options.DatabaseConnectionString))
        {
            return ValidateOptionsResult.Fail("Database connection string is required.");
        }

        return ValidateOptionsResult.Success;
    }
}
