using Microsoft.EntityFrameworkCore;
using SEOStore.Application.Interfaces.Repositories;
using SEOStore.Domain.Entities.Catalog;
using SEOStore.Infrastructure.Persistence;

namespace SEOStore.Infrastructure.Persistence.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .AsNoTracking()
            .Where(p => !p.IsDeleted)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, cancellationToken);
    }

    public async Task<Product?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .FirstOrDefaultAsync(p => p.Slug == slug && !p.IsDeleted, cancellationToken);
    }

    public async Task<List<Product>> GetPublishedAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .AsNoTracking()
            .Where(p => !p.IsDeleted && p.Published)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Product?> GetPublishedBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .FirstOrDefaultAsync(p => p.Slug == slug && !p.IsDeleted && p.Published, cancellationToken);
    }

    public async Task<List<Product>> GetPublishedByCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .AsNoTracking()
            .Where(p => !p.IsDeleted && p.Published && p.CategoryId == categoryId)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Product>> GetFeaturedPublishedAsync(int take, CancellationToken cancellationToken = default)
    {
        var featured = await _context.Products
            .AsNoTracking()
            .Where(p => !p.IsDeleted && p.Published && p.Featured)
            .OrderBy(p => p.Name)
            .Take(take)
            .ToListAsync(cancellationToken);

        if (featured.Count > 0)
            return featured;

        return await _context.Products
            .AsNoTracking()
            .Where(p => !p.IsDeleted && p.Published)
            .OrderByDescending(p => p.CreatedAt)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        for (var attempt = 1; attempt <= 6; attempt++)
        {
            try
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync(cancellationToken);
                return;
            }
            catch (DbUpdateException) when (attempt < 6)
            {
                _context.ChangeTracker.Clear();
                product.SetSlug($"{product.Slug}-{DateTime.UtcNow.Ticks}");
            }
        }
    }

    public async Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Product product, CancellationToken cancellationToken = default)
    {
        product.IsDeleted = true;
        product.SetSlug($"{product.Slug}-deleted-{product.Id}");
        product.UpdatedAt = DateTime.UtcNow;
        await UpdateAsync(product, cancellationToken);
    }

    public async Task<bool> ExistsBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .AnyAsync(p => p.Slug == slug, cancellationToken);
    }
}
