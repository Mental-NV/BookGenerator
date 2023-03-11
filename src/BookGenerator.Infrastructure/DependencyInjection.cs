using BookGenerator.Domain.Services;
using BookGenerator.Infrastructure.Books;
using Microsoft.Extensions.DependencyInjection;

namespace BookGenerator.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IBookCreater, BookCreater>();
        services.AddScoped<IBookConverter, BookConverter>();
        return services;
    }
}

