using MediatR;
using MyBankingApp.Application.Common;
using MyBankingApp.Application.DTOs;

namespace MyBankingApp.Application.Commands.Withdraw;

/// <summary>
/// Command to withdraw funds from an account.
/// </summary>
public record WithdrawCommand(
    Guid AccountId,
    decimal Amount,
    string Currency = "USD",
    string Description = "Withdrawal") : IRequest<Result<TransactionDto>>;
