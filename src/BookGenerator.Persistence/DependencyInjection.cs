using BookGenerator.Application.Abstractions.Data;
using BookGenerator.Domain.Services;
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
        services.AddScoped<IDbContext, BookDbContext>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        if (string.Equals(configuration["BookRepository"], "Test", StringComparison.OrdinalIgnoreCase))
        {
            services.AddScoped<IBookRepository, BookRepositoryInMemory>();
        }
        else
        {
            services.AddScoped<IBookRepository, BookRepository>();
        }
        
        return services;
    }
}
