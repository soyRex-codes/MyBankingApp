using MyBankingApp.Domain.Common;
using MyBankingApp.Domain.Enums;
using MyBankingApp.Domain.ValueObjects;

namespace MyBankingApp.Domain.Entities;

/// <summary>
/// Represents a financial transaction on an account.
/// </summary>
public class Transaction : BaseEntity
{
    public Guid AccountId { get; private set; }
    public Account Account { get; private set; } = null!;
    
    public TransactionType Type { get; private set; }
    public TransactionStatus Status { get; private set; }
    public Money Amount { get; private set; } = null!;
    public Money BalanceAfter { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    
    /// <summary>
    /// For transfers, this is the related account ID (source or destination).
    /// </summary>
    public Guid? RelatedAccountId { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
    
    public string ReferenceNumber { get; private set; } = null!;

    // For EF Core
    private Transaction() { }

    public Transaction(
        Guid accountId,
        TransactionType type,
        Money amount,
        Money balanceAfter,
        string description,
        Guid? relatedAccountId = null)
    {
        AccountId = accountId;
        Type = type;
        Amount = amount;
        BalanceAfter = balanceAfter;
        Description = description;
        RelatedAccountId = relatedAccountId;
        Status = TransactionStatus.Completed;
        CreatedAt = DateTime.UtcNow;
        ProcessedAt = DateTime.UtcNow;
        ReferenceNumber = GenerateReferenceNumber();
    }

    private static string GenerateReferenceNumber()
    {
        // Format: TXN-YYYYMMDD-XXXXXXXX
        var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
        var randomPart = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
        return $"TXN-{datePart}-{randomPart}";
    }
}
