﻿using BookGenerator.Domain.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookGenerator.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<BookContext>(options =>
            options.UseSqlServer(configuration["BOOKGENERATOR_CONNECTIONSTRING"]));
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
