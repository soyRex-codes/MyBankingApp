using Microsoft.EntityFrameworkCore;
using MyBankingApp.Domain.Entities;
using MyBankingApp.Domain.Interfaces;

namespace MyBankingApp.Infrastructure.Persistence.Repositories;

/// <summary>
/// Transaction repository implementation with pagination support.
/// </summary>
public class TransactionRepository : BaseRepository<Transaction>, ITransactionRepository
{
    public TransactionRepository(BankingDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Transaction>> GetByAccountIdAsync(
        Guid accountId,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.AccountId == accountId)
            .OrderByDescending(t => t.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetCountByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(t => t.AccountId == accountId, cancellationToken);
    }

    public async Task<Transaction?> GetByReferenceNumberAsync(string referenceNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(t => t.ReferenceNumber == referenceNumber, cancellationToken);
    }
}
