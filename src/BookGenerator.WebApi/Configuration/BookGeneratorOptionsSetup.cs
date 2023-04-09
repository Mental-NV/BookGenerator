using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace BookGenerator.WebApi.Configuration;

public class BookGeneratorOptionsSetup : IConfigureOptions<BookGeneratorOptions>
{
    private readonly IConfiguration configuration;

    public BookGeneratorOptionsSetup(IConfiguration configuration)
    {
        this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public void Configure(BookGeneratorOptions options)
    {
        configuration.GetSection(nameof(BookGeneratorOptions))
            .Bind(options);
    }
}
