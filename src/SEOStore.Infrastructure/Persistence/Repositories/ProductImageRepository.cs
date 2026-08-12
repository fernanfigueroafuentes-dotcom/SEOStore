using Microsoft.EntityFrameworkCore;
using SEOStore.Application.Interfaces.Repositories;
using SEOStore.Domain.Entities.Catalog;
using SEOStore.Infrastructure.Persistence;

namespace SEOStore.Infrastructure.Persistence.Repositories;

public class ProductImageRepository : IProductImageRepository
{
    private readonly ApplicationDbContext _context;

    public ProductImageRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductImage>> GetAllAsync(int productId, CancellationToken cancellationToken = default)
    {
        return await _context.ProductImages
            .AsNoTracking()
            .Where(x => x.ProductId == productId && !x.IsDeleted)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<ProductImage?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.ProductImages
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<ProductImage?> GetPrimaryAsync(int productId, CancellationToken cancellationToken = default)
    {
        return await _context.ProductImages
            .FirstOrDefaultAsync(x => x.ProductId == productId && x.IsPrimary && !x.IsDeleted, cancellationToken);
    }

    public async Task AddAsync(ProductImage productImage, CancellationToken cancellationToken = default)
    {
        await _context.ProductImages.AddAsync(productImage, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ProductImage productImage, CancellationToken cancellationToken = default)
    {
        _context.ProductImages.Update(productImage);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(ProductImage productImage, CancellationToken cancellationToken = default)
    {
        productImage.IsDeleted = true;
        productImage.UpdatedAt = DateTime.UtcNow;
        await UpdateAsync(productImage, cancellationToken);
    }
}
