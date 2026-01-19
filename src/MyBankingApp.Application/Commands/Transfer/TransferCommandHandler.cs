using MediatR;
using MyBankingApp.Application.Common;
using MyBankingApp.Application.DTOs;
using MyBankingApp.Domain.Exceptions;
using MyBankingApp.Domain.Interfaces;
using MyBankingApp.Domain.ValueObjects;

namespace MyBankingApp.Application.Commands.Transfer;

/// <summary>
/// Handler for TransferCommand.
/// Uses database transaction to ensure atomicity.
/// </summary>
public class TransferCommandHandler : IRequestHandler<TransferCommand, Result<TransferResultDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public TransferCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TransferResultDto>> Handle(TransferCommand request, CancellationToken cancellationToken)
    {
        // Validate accounts exist
        var sourceAccount = await _unitOfWork.Accounts.GetByIdAsync(request.SourceAccountId, cancellationToken);
        if (sourceAccount == null)
        {
            return Result<TransferResultDto>.Failure("Source account not found", "SOURCE_ACCOUNT_NOT_FOUND");
        }

        var destAccount = await _unitOfWork.Accounts.GetByIdAsync(request.DestinationAccountId, cancellationToken);
        if (destAccount == null)
        {
            return Result<TransferResultDto>.Failure("Destination account not found", "DEST_ACCOUNT_NOT_FOUND");
        }

        try
        {
            // Begin transaction for atomicity
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var money = new Money(request.Amount, request.Currency);
            var (outgoing, incoming) = sourceAccount.TransferTo(destAccount, money, request.Description);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            var outgoingDto = new TransactionDto(
                outgoing.Id,
                outgoing.ReferenceNumber,
                outgoing.Type.ToString(),
                outgoing.Status.ToString(),
                outgoing.Amount.Amount,
                outgoing.Amount.Currency,
                outgoing.BalanceAfter.Amount,
                outgoing.Description,
                outgoing.CreatedAt);

            var incomingDto = new TransactionDto(
                incoming.Id,
                incoming.ReferenceNumber,
                incoming.Type.ToString(),
                incoming.Status.ToString(),
                incoming.Amount.Amount,
                incoming.Amount.Currency,
                incoming.BalanceAfter.Amount,
                incoming.Description,
                incoming.CreatedAt);

            return Result<TransferResultDto>.Success(new TransferResultDto(outgoingDto, incomingDto));
        }
        catch (InsufficientFundsException ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result<TransferResultDto>.Failure(ex.Message, "INSUFFICIENT_FUNDS");
        }
        catch (AccountNotActiveException ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result<TransferResultDto>.Failure(ex.Message, "ACCOUNT_NOT_ACTIVE");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result<TransferResultDto>.Failure(ex.Message, "TRANSFER_FAILED");
        }
    }
}
