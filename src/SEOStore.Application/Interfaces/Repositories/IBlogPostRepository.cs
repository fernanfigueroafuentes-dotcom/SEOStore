using SEOStore.Domain.Entities.Content;

namespace SEOStore.Application.Interfaces.Repositories;

public interface IBlogPostRepository
{
    Task<List<BlogPost>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<List<BlogPost>> GetPublishedAsync(CancellationToken cancellationToken = default);

    Task<BlogPost?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<BlogPost?> GetPublishedBySlugAsync(string slug, CancellationToken cancellationToken = default);

    Task AddAsync(BlogPost post, CancellationToken cancellationToken = default);

    Task UpdateAsync(BlogPost post, CancellationToken cancellationToken = default);

    Task DeleteAsync(BlogPost post, CancellationToken cancellationToken = default);

    Task<bool> ExistsBySlugAsync(string slug, int? excludeId = null, CancellationToken cancellationToken = default);
}
