using Microsoft.EntityFrameworkCore;
using MyBankingApp.Domain.Entities;
using MyBankingApp.Domain.Interfaces;

namespace MyBankingApp.Infrastructure.Persistence.Repositories;

/// <summary>
/// Account repository implementation.
/// </summary>
public class AccountRepository : BaseRepository<Account>, IAccountRepository
{
    public AccountRepository(BankingDbContext context) : base(context)
    {
    }

    public async Task<Account?> GetByAccountNumberAsync(string accountNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber, cancellationToken);
    }

    public async Task<IEnumerable<Account>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Account?> GetWithTransactionsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.Transactions.OrderByDescending(t => t.CreatedAt).Take(50))
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }
}
