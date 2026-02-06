using BookGenerator.Domain.Services;
using BookGenerator.Infrastructure.Books;
using BookGenerator.Application.Abstractions.LLM;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenAI.Chat;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

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
        services.AddScoped<IBookConverter, PdfBookConverter>();
        
        InitializeQueryPdf();

        return services;
    }

    public static void InitializeQueryPdf()
    {
        QuestPDF.Settings.License = LicenseType.Community;
        QuestPDF.Settings.CheckIfAllTextGlyphsAreAvailable = false;

        var fonts = new string[]
        {
            "Assets/Fonts/NotoSansSC-VariableFont_wght.ttf"
        };

        foreach (var font in fonts)
        {
            using var stream = File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, font));
            FontManager.RegisterFontWithCustomName("Noto", stream);
        }
    }

    private static IServiceCollection AddOpenAIChatGpt(this IServiceCollection services, IConfiguration configuration)
    {
        var apiKey = configuration["BookGeneratorOptions:OpenAIApiKey"];

        // Register ChatClient as a singleton - it's thread-safe and should be reused
        services.AddSingleton<ChatClient>(new ChatClient(
            model: "gpt-4o-mini", // Using gpt-4o-mini for better quality and cost-efficiency
            apiKey: apiKey
        ));

        // Register the chat completion service
        services.AddSingleton<IChatCompletionService, ChatCompletionService>();

        return services;
    }
}

