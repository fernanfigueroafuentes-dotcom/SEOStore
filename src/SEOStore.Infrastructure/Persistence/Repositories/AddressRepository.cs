using Microsoft.EntityFrameworkCore;
using SEOStore.Application.Interfaces.Repositories;
using SEOStore.Domain.Entities.Users;
using SEOStore.Infrastructure.Persistence;

namespace SEOStore.Infrastructure.Persistence.Repositories;

public class AddressRepository : IAddressRepository
{
    private readonly ApplicationDbContext _context;

    public AddressRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<List<Address>> ListByUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        return _context.Addresses
            .Where(address => address.UserId == userId && !address.IsDeleted)
            .OrderByDescending(address => address.IsDefault)
            .ThenBy(address => address.Id)
            .ToListAsync(cancellationToken);
    }

    public Task<Address?> GetByIdAsync(int id, string userId, CancellationToken cancellationToken = default)
    {
        return _context.Addresses.FirstOrDefaultAsync(
            address => address.Id == id && address.UserId == userId && !address.IsDeleted,
            cancellationToken);
    }

    public async Task AddAsync(Address address, CancellationToken cancellationToken = default)
    {
        _context.Addresses.Add(address);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Address address, CancellationToken cancellationToken = default)
    {
        _context.Addresses.Update(address);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Address address, CancellationToken cancellationToken = default)
    {
        address.IsDeleted = true;
        address.UpdatedAt = DateTime.UtcNow;
        await UpdateAsync(address, cancellationToken);
    }
}
