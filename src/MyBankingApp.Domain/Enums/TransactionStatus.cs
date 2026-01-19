namespace MyBankingApp.Domain.Enums;

/// <summary>
/// Status of a transaction.
/// </summary>
public enum TransactionStatus
{
    /// <summary>
    /// Transaction is pending processing.
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Transaction completed successfully.
    /// </summary>
    Completed = 2,

    /// <summary>
    /// Transaction failed.
    /// </summary>
    Failed = 3,

    /// <summary>
    /// Transaction was reversed/refunded.
    /// </summary>
    Reversed = 4
}
