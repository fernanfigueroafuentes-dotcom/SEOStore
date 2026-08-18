using Microsoft.EntityFrameworkCore;
using SEOStore.Application.Interfaces.Repositories;
using SEOStore.Domain.Entities.Commerce;

namespace SEOStore.Infrastructure.Persistence.Repositories;

public class CartRepository : ICartRepository
{
    private readonly ApplicationDbContext _context;

    public CartRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Cart?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await CartWithItems()
            .FirstOrDefaultAsync(cart => cart.UserId == userId && !cart.IsDeleted, cancellationToken);
    }

    public async Task<Cart?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await CartWithItems()
            .FirstOrDefaultAsync(cart => cart.Id == id && !cart.IsDeleted, cancellationToken);
    }

    public async Task AddAsync(Cart cart, CancellationToken cancellationToken = default)
    {
        await _context.Carts.AddAsync(cart, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Cart cart, CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<Cart> CartWithItems() => _context.Carts
        .Include(cart => cart.Items.Where(item => !item.IsDeleted))
        .ThenInclude(item => item.Product);
}
