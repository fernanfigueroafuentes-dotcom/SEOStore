using SEOStore.Domain.Entities.Commerce;

namespace SEOStore.Application.Interfaces.Repositories;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<List<Order>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);

    Task<List<Order>> GetAllAsync(CancellationToken cancellationToken = default);

    Task AddAsync(Order order, CancellationToken cancellationToken = default);

    Task UpdateAsync(Order order, CancellationToken cancellationToken = default);
}
