using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SEOStore.Domain.Entities.Configuration;
using SEOStore.Domain.Entities.Content;
using SEOStore.Domain.Identity;
using SEOStore.Infrastructure.Persistence;

namespace SEOStore.Infrastructure.Identity;

public static class IdentityAndSiteSeeder
{
    public static async Task SeedAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        var configuration = services.GetRequiredService<IConfiguration>();
        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("IdentityAndSiteSeeder");
        var dbContext = services.GetRequiredService<ApplicationDbContext>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        if (!await roleManager.RoleExistsAsync(AppRoles.Admin))
            await roleManager.CreateAsync(new IdentityRole(AppRoles.Admin));

        var email = FirstValue(configuration, "ADMIN_EMAIL", "Admin:Email") ?? "admin@local.test";
        var password = FirstValue(configuration, "ADMIN_PASSWORD", "Admin:Password") ?? "Admin1234";

        var admin = await userManager.FindByEmailAsync(email);
        if (admin is null)
        {
            admin = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FirstName = "Admin"
            };

            var created = await userManager.CreateAsync(admin, password);
            if (!created.Succeeded)
            {
                logger.LogWarning(
                    "Could not create admin user {Email}: {Errors}",
                    email,
                    string.Join(", ", created.Errors.Select(error => error.Description)));
            }
            else
            {
                await userManager.AddToRoleAsync(admin, AppRoles.Admin);
                logger.LogInformation("Admin user {Email} created.", email);
            }
        }
        else if (!await userManager.IsInRoleAsync(admin, AppRoles.Admin))
        {
            await userManager.AddToRoleAsync(admin, AppRoles.Admin);
        }

        if (!dbContext.Settings.Any())
        {
            dbContext.Settings.Add(new Setting
            {
                SiteName = FirstValue(configuration, "SITE_NAME", "Site:SiteName") ?? "SEOStore",
                LogoUrl = FirstValue(configuration, "Site:LogoUrl") ?? string.Empty,
                FaviconUrl = FirstValue(configuration, "Site:FaviconUrl") ?? string.Empty,
                Phone = FirstValue(configuration, "Site:Phone") ?? string.Empty,
                Email = FirstValue(configuration, "Site:Email") ?? string.Empty,
                WhatsApp = FirstValue(configuration, "SITE_WHATSAPP", "Site:WhatsApp") ?? string.Empty,
                Facebook = FirstValue(configuration, "Site:Facebook") ?? string.Empty,
                Instagram = FirstValue(configuration, "Site:Instagram") ?? string.Empty,
                Address = FirstValue(configuration, "Site:Address") ?? string.Empty,
                PrimaryColor = FirstValue(configuration, "Site:PrimaryColor") ?? "#1a1a1a",
                SecondaryColor = FirstValue(configuration, "Site:SecondaryColor") ?? "#f6f6f6",
                GoogleAnalytics = FirstValue(configuration, "SITE_GA", "Site:GoogleAnalytics") ?? string.Empty,
                GoogleTagManager = FirstValue(configuration, "SITE_GTM", "Site:GoogleTagManager") ?? string.Empty,
                CreatedAt = DateTime.UtcNow
            });

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        if (!dbContext.Pages.Any(page => !page.IsDeleted))
        {
            dbContext.Pages.AddRange(
                new Page
                {
                    Title = "Nosotros",
                    Slug = "nosotros",
                    Content = "<p>Contá quiénes son, qué venden y por qué confiar en la marca. Editá esta página desde el admin.</p>",
                    Published = true,
                    MetaTitle = "Nosotros",
                    MetaDescription = "Conocé la marca, el equipo y la propuesta.",
                    Index = true,
                    Follow = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Page
                {
                    Title = "Envíos",
                    Slug = "envios",
                    Content = "<p>Explicá zonas de envío, demoras y costos. Editá esta página desde el admin.</p>",
                    Published = true,
                    MetaTitle = "Envíos",
                    MetaDescription = "Información de envíos, plazos y cobertura.",
                    Index = true,
                    Follow = true,
                    CreatedAt = DateTime.UtcNow
                });
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        await dbContext.Database.ExecuteSqlRawAsync(
            """UPDATE "Products" SET "Index" = TRUE, "Follow" = TRUE WHERE "MetaTitle" IS NULL""",
            cancellationToken: cancellationToken);
        await dbContext.Database.ExecuteSqlRawAsync(
            """UPDATE "Categories" SET "Index" = TRUE, "Follow" = TRUE WHERE "MetaTitle" IS NULL""",
            cancellationToken: cancellationToken);
    }

    private static string? FirstValue(IConfiguration configuration, params string[] keys)
    {
        foreach (var key in keys)
        {
            var value = configuration[key];
            if (!string.IsNullOrWhiteSpace(value))
                return value.Trim();
        }

        return null;
    }
}
