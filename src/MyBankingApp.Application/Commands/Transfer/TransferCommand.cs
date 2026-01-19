using MediatR;
using MyBankingApp.Application.Common;
using MyBankingApp.Application.DTOs;

namespace MyBankingApp.Application.Commands.Transfer;

/// <summary>
/// Command to transfer funds between accounts.
/// </summary>
public record TransferCommand(
    Guid SourceAccountId,
    Guid DestinationAccountId,
    decimal Amount,
    string Currency = "USD",
    string? Description = null) : IRequest<Result<TransferResultDto>>;

/// <summary>
/// DTO for transfer result containing both transactions.
/// </summary>
public record TransferResultDto(
    TransactionDto OutgoingTransaction,
    TransactionDto IncomingTransaction);
