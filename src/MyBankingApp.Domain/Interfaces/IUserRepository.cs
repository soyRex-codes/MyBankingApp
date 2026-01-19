using MyBankingApp.Domain.Entities;

namespace MyBankingApp.Domain.Interfaces;

/// <summary>
/// Repository interface for User entity.
/// </summary>
public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetWithAccountsAsync(Guid id, CancellationToken cancellationToken = default);
}
