using BookGenerator.Domain.Services;
using BookGenerator.Infrastructure.Books;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenAI.Extensions;
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
        services.AddOpenAIService(settings =>
        {
            settings.ApiKey = configuration["BookGeneratorOptions:OpenAIApiKey"];
            settings.Organization = configuration["BookGeneratorOptions:OpenAIOrganization"];
        });
        services.AddHttpClient();
        return services;
    }
}

