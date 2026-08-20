using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SEOStore.Application.Interfaces.Repositories;
using SEOStore.Application.Interfaces.Services;
using SEOStore.Infrastructure.Identity;
using SEOStore.Infrastructure.Persistence;
using SEOStore.Infrastructure.Persistence.Repositories;
using SEOStore.Infrastructure.Services;

namespace SEOStore.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredUniqueChars = 1;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/cuenta/ingresar";
            options.AccessDeniedPath = "/cuenta/ingresar";
            options.Cookie.Name = "SEOStore.Auth";
            options.SlidingExpiration = true;
            options.Events.OnRedirectToLogin = CookieAuthJson.RedirectToLogin;
            options.Events.OnRedirectToAccessDenied = CookieAuthJson.RedirectToAccessDenied;
        });

        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IBrandRepository, BrandRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductImageRepository, ProductImageRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IAddressRepository, AddressRepository>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<ISettingRepository, SettingRepository>();
        services.AddScoped<IPageRepository, PageRepository>();
        services.AddScoped<IBlogPostRepository, BlogPostRepository>();
        services.AddScoped<IBannerRepository, BannerRepository>();
        services.AddScoped<ISlugRedirectRepository, SlugRedirectRepository>();
        services.AddScoped<ISlugRegistry, SlugRegistry>();
        services.AddScoped<IImageStorageService>(provider =>
        {
            var config = provider.GetRequiredService<IConfiguration>();
            var environment = provider.GetRequiredService<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>();
            var logger = provider.GetRequiredService<ILogger<CloudinaryImageStorageService>>();
            var localStorage = new LocalImageStorageService(environment);

            if (!CloudinarySettings.TryRead(config, out var cloudName, out _, out _))
            {
                logger.LogWarning("Cloudinary is not configured. Product photos will be stored in wwwroot/uploads.");
                return localStorage;
            }

            logger.LogInformation("Cloudinary configured for cloud {CloudName}.", cloudName);
            return new CloudinaryImageStorageService(config);
        });

        return services;
    }
}
