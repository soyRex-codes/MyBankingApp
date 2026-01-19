namespace MyBankingApp.Application.DTOs;

/// <summary>
/// DTO for account information.
/// </summary>
public record AccountDto(
    Guid Id,
    string AccountNumber,
    string Type,
    string Status,
    decimal Balance,
    string Currency,
    DateTime CreatedAt);
