namespace MyBankingApp.Application.DTOs;

/// <summary>
/// DTO for user information.
/// </summary>
public record UserDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    bool IsEmailVerified,
    DateTime CreatedAt);
