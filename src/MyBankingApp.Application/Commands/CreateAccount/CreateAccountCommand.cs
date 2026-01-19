using MediatR;
using MyBankingApp.Application.Common;
using MyBankingApp.Application.DTOs;
using MyBankingApp.Domain.Enums;

namespace MyBankingApp.Application.Commands.CreateAccount;

/// <summary>
/// Command to create a new bank account.
/// </summary>
public record CreateAccountCommand(
    Guid UserId,
    AccountType Type,
    string Currency = "USD") : IRequest<Result<AccountDto>>;
