using Microsoft.EntityFrameworkCore;
using MyBankingApp.Domain.Entities;
using MyBankingApp.Domain.Interfaces;

namespace MyBankingApp.Infrastructure.Persistence.Repositories;

/// <summary>
/// User repository implementation.
/// </summary>
public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(BankingDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant(), cancellationToken);
    }

    public async Task<bool> ExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(u => u.Email == email.ToLowerInvariant(), cancellationToken);
    }

    public async Task<User?> GetWithAccountsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.Accounts)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }
}
