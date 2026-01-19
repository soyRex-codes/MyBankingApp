namespace MyBankingApp.Application.DTOs;

/// <summary>
/// DTO for transaction information.
/// </summary>
public record TransactionDto(
    Guid Id,
    string ReferenceNumber,
    string Type,
    string Status,
    decimal Amount,
    string Currency,
    decimal BalanceAfter,
    string Description,
    DateTime CreatedAt);
