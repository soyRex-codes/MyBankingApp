using MyBankingApp.Domain.Common;
using MyBankingApp.Domain.Enums;

namespace MyBankingApp.Domain.Events;

/// <summary>
/// Event raised when a new account is created.
/// </summary>
public sealed class AccountCreatedEvent : IDomainEvent
{
    public Guid AccountId { get; }
    public Guid UserId { get; }
    public AccountType AccountType { get; }
    public DateTime OccurredOn { get; }

    public AccountCreatedEvent(Guid accountId, Guid userId, AccountType accountType)
    {
        AccountId = accountId;
        UserId = userId;
        AccountType = accountType;
        OccurredOn = DateTime.UtcNow;
    }
}
