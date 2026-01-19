using MyBankingApp.Domain.Entities;

namespace MyBankingApp.Domain.Interfaces;

/// <summary>
/// Repository interface for Account entity with specialized queries.
/// </summary>
public interface IAccountRepository : IRepository<Account>
{
    Task<Account?> GetByAccountNumberAsync(string accountNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<Account>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Account?> GetWithTransactionsAsync(Guid id, CancellationToken cancellationToken = default);
}
