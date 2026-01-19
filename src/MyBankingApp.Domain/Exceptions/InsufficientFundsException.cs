using MyBankingApp.Domain.ValueObjects;

namespace MyBankingApp.Domain.Exceptions;

/// <summary>
/// Exception thrown when an account has insufficient funds for a transaction.
/// </summary>
public sealed class InsufficientFundsException : DomainException
{
    public Guid AccountId { get; }
    public Money RequestedAmount { get; }
    public Money AvailableBalance { get; }

    public InsufficientFundsException(Guid accountId, Money requestedAmount, Money availableBalance)
        : base($"Account {accountId} has insufficient funds. Requested: {requestedAmount}, Available: {availableBalance}")
    {
        AccountId = accountId;
        RequestedAmount = requestedAmount;
        AvailableBalance = availableBalance;
    }
}
