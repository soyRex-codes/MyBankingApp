using MyBankingApp.Domain.Common;
using MyBankingApp.Domain.ValueObjects;

namespace MyBankingApp.Domain.Events;

/// <summary>
/// Event raised when funds are deposited into an account.
/// </summary>
public sealed class FundsDepositedEvent : IDomainEvent
{
    public Guid AccountId { get; }
    public Money Amount { get; }
    public Money NewBalance { get; }
    public DateTime OccurredOn { get; }

    public FundsDepositedEvent(Guid accountId, Money amount, Money newBalance)
    {
        AccountId = accountId;
        Amount = amount;
        NewBalance = newBalance;
        OccurredOn = DateTime.UtcNow;
    }
}
