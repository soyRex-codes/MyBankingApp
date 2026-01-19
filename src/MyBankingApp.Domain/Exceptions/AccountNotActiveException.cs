namespace MyBankingApp.Domain.Exceptions;

/// <summary>
/// Exception thrown when an operation is attempted on an inactive account.
/// </summary>
public sealed class AccountNotActiveException : DomainException
{
    public Guid AccountId { get; }

    public AccountNotActiveException(Guid accountId)
        : base($"Account {accountId} is not active and cannot process transactions")
    {
        AccountId = accountId;
    }
}
