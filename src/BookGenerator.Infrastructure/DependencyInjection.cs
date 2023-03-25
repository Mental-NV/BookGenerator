using BookGenerator.Domain.Services;
using BookGenerator.Infrastructure.Books;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenAI.GPT3.Extensions;

namespace BookGenerator.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        if (string.Equals(configuration["Model"], "Test"))
        {
            services.AddScoped<IBookCreater, BookCreaterInMemory>();
        }
        else
        {
            services.AddScoped<IBookCreater, BookCreaterChatGpt>();
        }
        services.AddOpenAIChatGpt(configuration);
        services.AddScoped<IBookConverter, BookConverter>();
        return services;
    }

    private static IServiceCollection AddOpenAIChatGpt(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenAIService(settings =>
        {
            settings.ApiKey = configuration["OPENAI_APIKEY"];
            settings.Organization = configuration["OPENAI_ORG"];
        });
        services.AddHttpClient();
        return services;
    }
}

