using MediatR;
using MyBankingApp.Application.Common;
using MyBankingApp.Application.DTOs;

namespace MyBankingApp.Application.Commands.Deposit;

/// <summary>
/// Command to deposit funds into an account.
/// </summary>
public record DepositCommand(
    Guid AccountId,
    decimal Amount,
    string Currency = "USD",
    string Description = "Deposit") : IRequest<Result<TransactionDto>>;
