using BookGenerator.Domain.Services;
using BookGenerator.Infrastructure.Books;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenAI.GPT3.Extensions;

namespace BookGenerator.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        if (environment.IsProduction())
        {
            services.AddScoped<IBookCreater, BookCreaterChatGpt>();
        }
        else
        {
            services.AddScoped<IBookCreater, BookCreaterInMemory>();
        }
        services.AddOpenAIChatGpt(configuration);
        services.AddScoped<IBookRepository, BookRepositoryInMemory>();
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

