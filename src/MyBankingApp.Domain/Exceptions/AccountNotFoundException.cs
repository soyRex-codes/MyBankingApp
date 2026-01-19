namespace MyBankingApp.Domain.Exceptions;

/// <summary>
/// Exception thrown when an account is not found.
/// </summary>
public sealed class AccountNotFoundException : DomainException
{
    public Guid? AccountId { get; }
    public string? AccountNumber { get; }

    public AccountNotFoundException(Guid accountId)
        : base($"Account with ID {accountId} was not found")
    {
        AccountId = accountId;
    }

    public AccountNotFoundException(string accountNumber)
        : base($"Account with number {accountNumber} was not found")
    {
        AccountNumber = accountNumber;
    }
}
