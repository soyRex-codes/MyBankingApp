using MediatR;
using MyBankingApp.Application.Common;
using MyBankingApp.Application.DTOs;
using MyBankingApp.Domain.Interfaces;
using MyBankingApp.Domain.ValueObjects;

namespace MyBankingApp.Application.Commands.Deposit;

/// <summary>
/// Handler for DepositCommand.
/// </summary>
public class DepositCommandHandler : IRequestHandler<DepositCommand, Result<TransactionDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public DepositCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TransactionDto>> Handle(DepositCommand request, CancellationToken cancellationToken)
    {
        var account = await _unitOfWork.Accounts.GetByIdAsync(request.AccountId, cancellationToken);
        if (account == null)
        {
            return Result<TransactionDto>.Failure("Account not found", "ACCOUNT_NOT_FOUND");
        }

        try
        {
            var money = new Money(request.Amount, request.Currency);
            var transaction = account.Deposit(money, request.Description);
            
            // Explicitly add transaction to DbContext so EF Core tracks it as a new entity
            // This is needed because the transaction is added to Account's backing field collection
            // which EF Core may not detect as a new entity without explicit tracking
            await _unitOfWork.Transactions.AddAsync(transaction, cancellationToken);
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = new TransactionDto(
                transaction.Id,
                transaction.ReferenceNumber,
                transaction.Type.ToString(),
                transaction.Status.ToString(),
                transaction.Amount.Amount,
                transaction.Amount.Currency,
                transaction.BalanceAfter.Amount,
                transaction.Description,
                transaction.CreatedAt);

            return Result<TransactionDto>.Success(dto);
        }
        catch (Exception ex)
        {
            return Result<TransactionDto>.Failure(ex.Message, "DEPOSIT_FAILED");
        }
    }
}
