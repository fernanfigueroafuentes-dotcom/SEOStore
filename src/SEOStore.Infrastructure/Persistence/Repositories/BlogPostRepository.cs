using Microsoft.EntityFrameworkCore;
using SEOStore.Application.Interfaces.Repositories;
using SEOStore.Domain.Entities.Content;
using SEOStore.Infrastructure.Persistence;

namespace SEOStore.Infrastructure.Persistence.Repositories;

public class BlogPostRepository : IBlogPostRepository
{
    private readonly ApplicationDbContext _context;

    public BlogPostRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<BlogPost>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.BlogPosts
            .AsNoTracking()
            .Where(post => !post.IsDeleted)
            .OrderBy(post => post.DisplayOrder)
            .ThenByDescending(post => post.PublishedAt ?? post.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<BlogPost>> GetPublishedAsync(CancellationToken cancellationToken = default)
    {
        return await _context.BlogPosts
            .AsNoTracking()
            .Where(post => !post.IsDeleted && post.Published)
            .OrderBy(post => post.DisplayOrder)
            .ThenByDescending(post => post.PublishedAt ?? post.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<BlogPost?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.BlogPosts.FirstOrDefaultAsync(post => post.Id == id && !post.IsDeleted, cancellationToken);
    }

    public async Task<BlogPost?> GetPublishedBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _context.BlogPosts.FirstOrDefaultAsync(
            post => post.Slug == slug && !post.IsDeleted && post.Published,
            cancellationToken);
    }

    public async Task AddAsync(BlogPost post, CancellationToken cancellationToken = default)
    {
        _context.BlogPosts.Add(post);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(BlogPost post, CancellationToken cancellationToken = default)
    {
        _context.BlogPosts.Update(post);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(BlogPost post, CancellationToken cancellationToken = default)
    {
        post.IsDeleted = true;
        post.Slug = $"{post.Slug}-deleted-{post.Id}";
        post.UpdatedAt = DateTime.UtcNow;
        await UpdateAsync(post, cancellationToken);
    }

    public async Task<bool> ExistsBySlugAsync(string slug, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        return await _context.BlogPosts.AnyAsync(
            post => post.Slug == slug && !post.IsDeleted && (excludeId == null || post.Id != excludeId),
            cancellationToken);
    }
}
