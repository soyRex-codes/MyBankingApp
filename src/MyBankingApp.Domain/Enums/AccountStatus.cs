namespace MyBankingApp.Domain.Enums;

/// <summary>
/// Status of a bank account.
/// </summary>
public enum AccountStatus
{
    /// <summary>
    /// Account is active and operational.
    /// </summary>
    Active = 1,

    /// <summary>
    /// Account is temporarily frozen.
    /// </summary>
    Frozen = 2,

    /// <summary>
    /// Account has been closed.
    /// </summary>
    Closed = 3
}
