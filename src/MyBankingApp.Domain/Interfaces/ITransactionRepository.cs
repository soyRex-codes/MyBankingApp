using MyBankingApp.Domain.Entities;

namespace MyBankingApp.Domain.Interfaces;

/// <summary>
/// Repository interface for Transaction entity with filtering capabilities.
/// </summary>
public interface ITransactionRepository : IRepository<Transaction>
{
    Task<IEnumerable<Transaction>> GetByAccountIdAsync(
        Guid accountId, 
        int pageNumber = 1, 
        int pageSize = 20, 
        CancellationToken cancellationToken = default);
    
    Task<int> GetCountByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
    
    Task<Transaction?> GetByReferenceNumberAsync(string referenceNumber, CancellationToken cancellationToken = default);
}
