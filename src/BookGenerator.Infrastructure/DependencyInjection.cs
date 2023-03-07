using Microsoft.Extensions.DependencyInjection;

namespace BookGenerator.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}

