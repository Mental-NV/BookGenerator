namespace BookGenerator.WebApi.Configuration;

public class BookGeneratorOptions
{
    public string OpenAIApiKey { get; set; }
    public string OpenAIOrganization { get; set; }
    public string DatabaseConnectionString { get; set; }
}
