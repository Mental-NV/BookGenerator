using BookGenerator.Application.Abstractions.Data;
using BookGenerator.Domain.Repositories;
using BookGenerator.Persistence.Books;
using BookGenerator.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookGenerator.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<BookDbContext>(options =>
            options.UseSqlServer(configuration["BOOKGENERATOR_CONNECTIONSTRING"]));
        services.AddScoped<IDbContext>(serviceProvider => serviceProvider.GetRequiredService<BookDbContext>());
        services.AddScoped<IUnitOfWork>(serviceProvider => serviceProvider.GetRequiredService<BookDbContext>());
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IChapterRepository, ChapterRepository>();
        services.AddScoped<IProgressRepository, ProgressRepository>();
        
        return services;
    }
}
