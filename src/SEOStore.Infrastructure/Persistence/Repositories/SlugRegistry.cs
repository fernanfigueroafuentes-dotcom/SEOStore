using Microsoft.EntityFrameworkCore;
using SEOStore.Application.Common;
using SEOStore.Application.Interfaces.Repositories;
using SEOStore.Infrastructure.Persistence;

namespace SEOStore.Infrastructure.Persistence.Repositories;

public class SlugRegistry : ISlugRegistry
{
    private readonly ApplicationDbContext _context;

    public SlugRegistry(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsTakenAsync(string slug, SlugKind? excludeKind, int? excludeId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(slug))
            return true;

        if (await ProductTakenAsync(slug, Exclude(excludeKind, SlugKind.Product, excludeId), cancellationToken))
            return true;

        if (await CategoryTakenAsync(slug, Exclude(excludeKind, SlugKind.Category, excludeId), cancellationToken))
            return true;

        if (await PageTakenAsync(slug, Exclude(excludeKind, SlugKind.Page, excludeId), cancellationToken))
            return true;

        return await PostTakenAsync(slug, Exclude(excludeKind, SlugKind.BlogPost, excludeId), cancellationToken);
    }

    public async Task<bool> IsLivePathAsync(string path, CancellationToken cancellationToken = default)
    {
        if (!PublicSlug.TryParse(path, out var kind, out var slug))
            return false;

        return kind switch
        {
            SlugKind.Product => await _context.Products.AnyAsync(
                product => product.Slug == slug && !product.IsDeleted && product.Published,
                cancellationToken),
            SlugKind.Category => await _context.Categories.AnyAsync(
                category => category.Slug == slug && !category.IsDeleted && category.Published,
                cancellationToken),
            SlugKind.Page => await _context.Pages.AnyAsync(
                page => page.Slug == slug && !page.IsDeleted && page.Published,
                cancellationToken),
            SlugKind.BlogPost => await _context.BlogPosts.AnyAsync(
                post => post.Slug == slug && !post.IsDeleted && post.Published,
                cancellationToken),
            _ => false
        };
    }

    private static int? Exclude(SlugKind? excludeKind, SlugKind kind, int? excludeId)
        => excludeKind == kind ? excludeId : null;

    private Task<bool> ProductTakenAsync(string slug, int? excludeId, CancellationToken cancellationToken)
        => _context.Products.AnyAsync(
            product => product.Slug == slug && !product.IsDeleted && (excludeId == null || product.Id != excludeId),
            cancellationToken);

    private Task<bool> CategoryTakenAsync(string slug, int? excludeId, CancellationToken cancellationToken)
        => _context.Categories.AnyAsync(
            category => category.Slug == slug && !category.IsDeleted && (excludeId == null || category.Id != excludeId),
            cancellationToken);

    private Task<bool> PageTakenAsync(string slug, int? excludeId, CancellationToken cancellationToken)
        => _context.Pages.AnyAsync(
            page => page.Slug == slug && !page.IsDeleted && (excludeId == null || page.Id != excludeId),
            cancellationToken);

    private Task<bool> PostTakenAsync(string slug, int? excludeId, CancellationToken cancellationToken)
        => _context.BlogPosts.AnyAsync(
            post => post.Slug == slug && !post.IsDeleted && (excludeId == null || post.Id != excludeId),
            cancellationToken);
}
