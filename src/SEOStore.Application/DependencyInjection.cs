using Microsoft.Extensions.DependencyInjection;
using SEOStore.Application.Interfaces.Services;
using SEOStore.Application.Services;

namespace SEOStore.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICategoryService, CategoryService>();
        return services;
    }
}
