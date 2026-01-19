namespace MyBankingApp.Domain.Enums;

/// <summary>
/// Types of financial transactions.
/// </summary>
public enum TransactionType
{
    /// <summary>
    /// Money deposited into account.
    /// </summary>
    Deposit = 1,

    /// <summary>
    /// Money withdrawn from account.
    /// </summary>
    Withdrawal = 2,

    /// <summary>
    /// Transfer to another account (outgoing).
    /// </summary>
    TransferOut = 3,

    /// <summary>
    /// Transfer from another account (incoming).
    /// </summary>
    TransferIn = 4
}
