using BookGenerator.Application.Abstractions.Data;
using BookGenerator.Domain.Repositories;
using BookGenerator.Persistence.Books;
using BookGenerator.Persistence.Interceptors;
using BookGenerator.Persistence.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BookGenerator.Persistence;

public static class DependencyInjection
{
    private const string DefaultDevelopmentSqliteConnectionString = "Data Source=App_Data/bookgenerator.db;Cache=Shared;Pooling=True";

    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ConvertDomainEventsToOutboxMessagesInterceptor>();
        services.AddDbContext<BookDbContext>((sp, options) =>
            {
                var interceptor = sp.GetRequiredService<ConvertDomainEventsToOutboxMessagesInterceptor>();
                string configuredConnectionString = configuration["BookGeneratorOptions:DatabaseConnectionString"]
                    ?? throw new InvalidOperationException("BookGeneratorOptions:DatabaseConnectionString is not configured.");
                bool isDevelopment =
                    string.Equals(configuration["ASPNETCORE_ENVIRONMENT"], "Development", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(configuration["DOTNET_ENVIRONMENT"], "Development", StringComparison.OrdinalIgnoreCase);
                string connectionString = ResolveSqliteConnectionString(configuredConnectionString, isDevelopment);

                EnsureSqliteDatabaseDirectoryExists(connectionString);

                options.UseSqlite(connectionString)
                    .AddInterceptors(interceptor);
            });
        services.AddScoped<IDbContext>(serviceProvider => serviceProvider.GetRequiredService<BookDbContext>());
        services.AddScoped<IUnitOfWork>(serviceProvider => serviceProvider.GetRequiredService<BookDbContext>());
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IChapterRepository, ChapterRepository>();
        services.AddScoped<IProgressRepository, ProgressRepository>();
        
        return services;
    }

    public static async Task ApplyPersistenceMigrationsAsync(this IServiceProvider services, ILogger logger, CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("Applying database migrations...");
            using IServiceScope scope = services.CreateScope();
            BookDbContext dbContext = scope.ServiceProvider.GetRequiredService<BookDbContext>();
            await dbContext.Database.MigrateAsync(cancellationToken);
            logger.LogInformation("Database migrations applied successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to apply database migrations during startup.");
            throw;
        }
    }

    private static string ResolveSqliteConnectionString(string configuredConnectionString, bool isDevelopment)
    {
        if (IsValidSqliteConnectionString(configuredConnectionString))
        {
            return configuredConnectionString;
        }

        if (isDevelopment && LooksLikeSqlServerConnectionString(configuredConnectionString))
        {
            return DefaultDevelopmentSqliteConnectionString;
        }

        throw new InvalidOperationException(
            "BookGeneratorOptions:DatabaseConnectionString must be a SQLite connection string. Example: " +
            "Data Source=/home/data/bookgenerator/bookgenerator.db;Cache=Shared;Pooling=True");
    }

    private static void EnsureSqliteDatabaseDirectoryExists(string connectionString)
    {
        if (!TryBuildSqliteConnectionString(connectionString, out var builder))
        {
            return;
        }

        string dataSource = builder.DataSource;

        if (string.IsNullOrWhiteSpace(dataSource) ||
            dataSource.Equals(":memory:", StringComparison.OrdinalIgnoreCase) ||
            dataSource.StartsWith("file:", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        string fullPath = Path.GetFullPath(dataSource);
        string directory = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    private static bool IsValidSqliteConnectionString(string connectionString)
        => TryBuildSqliteConnectionString(connectionString, out _);

    private static bool TryBuildSqliteConnectionString(string connectionString, out SqliteConnectionStringBuilder builder)
    {
        try
        {
            builder = new SqliteConnectionStringBuilder(connectionString);
            return true;
        }
        catch (ArgumentException)
        {
            builder = null!;
            return false;
        }
    }

    private static bool LooksLikeSqlServerConnectionString(string connectionString)
    {
        string normalized = connectionString.ToLowerInvariant();
        return normalized.Contains("server=") ||
               normalized.Contains("data source=") ||
               normalized.Contains("initial catalog=") ||
               normalized.Contains("integrated security=");
    }
}
