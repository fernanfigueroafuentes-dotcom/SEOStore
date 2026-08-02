using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SEOStore.Domain.Entities.Catalog;
using SEOStore.Domain.Entities.Commerce;
using SEOStore.Domain.Entities.Configuration;
using SEOStore.Domain.Entities.Content;
using SEOStore.Domain.Entities.Integrations;
using SEOStore.Infrastructure.Identity;

namespace SEOStore.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Catalog
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();

    // Commerce
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    // Content
    public DbSet<BlogPost> BlogPosts => Set<BlogPost>();
    public DbSet<Page> Pages => Set<Page>();
    public DbSet<Banner> Banners => Set<Banner>();

    // Configuration
    public DbSet<Setting> Settings => Set<Setting>();

    // Integrations
    public DbSet<MercadoLibreAuth> MercadoLibreAuths => Set<MercadoLibreAuth>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}