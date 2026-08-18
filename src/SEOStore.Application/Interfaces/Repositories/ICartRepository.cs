using SEOStore.Domain.Entities.Commerce;

namespace SEOStore.Application.Interfaces.Repositories;

public interface ICartRepository
{
    Task<Cart?> GetByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<Cart?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Cart cart,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Cart cart,
        CancellationToken cancellationToken = default);
}
