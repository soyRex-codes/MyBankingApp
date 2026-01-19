namespace MyBankingApp.Domain.Enums;

/// <summary>
/// Types of bank accounts supported by the system.
/// </summary>
public enum AccountType
{
    /// <summary>
    /// Standard checking account for everyday transactions.
    /// </summary>
    Checking = 1,

    /// <summary>
    /// Savings account with interest earnings.
    /// </summary>
    Savings = 2,

    /// <summary>
    /// Money market account with higher interest rates.
    /// </summary>
    MoneyMarket = 3
}
