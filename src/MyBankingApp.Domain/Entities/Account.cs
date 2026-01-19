using MyBankingApp.Domain.Common;
using MyBankingApp.Domain.Enums;
using MyBankingApp.Domain.Events;
using MyBankingApp.Domain.Exceptions;
using MyBankingApp.Domain.ValueObjects;

namespace MyBankingApp.Domain.Entities;

/// <summary>
/// Represents a bank account with rich domain behavior.
/// Business logic is encapsulated within the entity (DDD pattern).
/// </summary>
public class Account : AuditableEntity
{
    public string AccountNumber { get; private set; } = null!;
    public AccountType Type { get; private set; }
    public AccountStatus Status { get; private set; }
    public Money Balance { get; private set; } = null!;
    
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    private readonly List<Transaction> _transactions = [];
    public IReadOnlyCollection<Transaction> Transactions => _transactions.AsReadOnly();

    // For EF Core
    private Account() { }

    public Account(Guid userId, AccountType type, string currency = "USD")
    {
        UserId = userId;
        Type = type;
        Status = AccountStatus.Active;
        Balance = Money.Zero(currency);
        AccountNumber = GenerateAccountNumber();
        
        AddDomainEvent(new AccountCreatedEvent(Id, userId, type));
    }

    /// <summary>
    /// Deposit funds into the account.
    /// </summary>
    public Transaction Deposit(Money amount, string description = "Deposit")
    {
        EnsureAccountIsActive();
        
        if (amount.Amount <= 0)
            throw new ArgumentException("Deposit amount must be positive", nameof(amount));

        Balance += amount;
        
        // Create a new Money instance for BalanceAfter to avoid EF Core owned type tracking conflict
        var balanceAfter = new Money(Balance.Amount, Balance.Currency);
        
        var transaction = new Transaction(
            Id,
            TransactionType.Deposit,
            amount,
            balanceAfter,
            description);
        
        _transactions.Add(transaction);
        AddDomainEvent(new FundsDepositedEvent(Id, amount, Balance));
        
        return transaction;
    }

    /// <summary>
    /// Withdraw funds from the account.
    /// </summary>
    public Transaction Withdraw(Money amount, string description = "Withdrawal")
    {
        EnsureAccountIsActive();
        
        if (amount.Amount <= 0)
            throw new ArgumentException("Withdrawal amount must be positive", nameof(amount));

        if (amount > Balance)
            throw new InsufficientFundsException(Id, amount, Balance);

        Balance -= amount;
        
        // Create a new Money instance for BalanceAfter to avoid EF Core owned type tracking conflict
        var balanceAfter = new Money(Balance.Amount, Balance.Currency);
        
        var transaction = new Transaction(
            Id,
            TransactionType.Withdrawal,
            amount,
            balanceAfter,
            description);
        
        _transactions.Add(transaction);
        AddDomainEvent(new FundsWithdrawnEvent(Id, amount, Balance));
        
        return transaction;
    }

    /// <summary>
    /// Transfer funds to another account.
    /// </summary>
    public (Transaction outgoing, Transaction incoming) TransferTo(Account destinationAccount, Money amount, string? description = null)
    {
        EnsureAccountIsActive();
        destinationAccount.EnsureAccountIsActive();
        
        if (destinationAccount.Id == Id)
            throw new InvalidOperationException("Cannot transfer to the same account");

        var transferDescription = description ?? $"Transfer to {destinationAccount.AccountNumber}";
        var receiveDescription = $"Transfer from {AccountNumber}";

        // Withdraw from source
        if (amount > Balance)
            throw new InsufficientFundsException(Id, amount, Balance);

        Balance -= amount;
        // Create new Money instances for BalanceAfter to avoid EF Core owned type tracking conflict
        var sourceBalanceAfter = new Money(Balance.Amount, Balance.Currency);
        var outgoingTransaction = new Transaction(
            Id,
            TransactionType.TransferOut,
            amount,
            sourceBalanceAfter,
            transferDescription,
            destinationAccount.Id);
        _transactions.Add(outgoingTransaction);

        // Deposit to destination
        destinationAccount.Balance += amount;
        var destBalanceAfter = new Money(destinationAccount.Balance.Amount, destinationAccount.Balance.Currency);
        var incomingTransaction = new Transaction(
            destinationAccount.Id,
            TransactionType.TransferIn,
            amount,
            destBalanceAfter,
            receiveDescription,
            Id);
        destinationAccount._transactions.Add(incomingTransaction);

        AddDomainEvent(new FundsTransferredEvent(Id, destinationAccount.Id, amount));

        return (outgoingTransaction, incomingTransaction);
    }

    /// <summary>
    /// Freeze the account (prevent transactions).
    /// </summary>
    public void Freeze()
    {
        if (Status == AccountStatus.Closed)
            throw new InvalidOperationException("Cannot freeze a closed account");
        
        Status = AccountStatus.Frozen;
    }

    /// <summary>
    /// Unfreeze the account.
    /// </summary>
    public void Unfreeze()
    {
        if (Status == AccountStatus.Closed)
            throw new InvalidOperationException("Cannot unfreeze a closed account");
        
        Status = AccountStatus.Active;
    }

    /// <summary>
    /// Close the account. Balance must be zero.
    /// </summary>
    public void Close()
    {
        if (Balance.Amount != 0)
            throw new InvalidOperationException("Cannot close account with non-zero balance");
        
        Status = AccountStatus.Closed;
    }

    private void EnsureAccountIsActive()
    {
        if (Status != AccountStatus.Active)
            throw new AccountNotActiveException(Id);
    }

    private static string GenerateAccountNumber()
    {
        // Generate a 12-digit account number
        var random = new Random();
        return string.Concat(Enumerable.Range(0, 12).Select(_ => random.Next(0, 10)));
    }
}
