using MediatR;
using MyBankingApp.Application.Common;
using MyBankingApp.Application.DTOs;
using MyBankingApp.Domain.Exceptions;
using MyBankingApp.Domain.Interfaces;
using MyBankingApp.Domain.ValueObjects;

namespace MyBankingApp.Application.Commands.Withdraw;

/// <summary>
/// Handler for WithdrawCommand.
/// </summary>
public class WithdrawCommandHandler : IRequestHandler<WithdrawCommand, Result<TransactionDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public WithdrawCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TransactionDto>> Handle(WithdrawCommand request, CancellationToken cancellationToken)
    {
        var account = await _unitOfWork.Accounts.GetByIdAsync(request.AccountId, cancellationToken);
        if (account == null)
        {
            return Result<TransactionDto>.Failure("Account not found", "ACCOUNT_NOT_FOUND");
        }

        try
        {
            var money = new Money(request.Amount, request.Currency);
            var transaction = account.Withdraw(money, request.Description);
            
            // Explicitly add transaction to DbContext so EF Core tracks it as a new entity
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
        catch (InsufficientFundsException ex)
        {
            return Result<TransactionDto>.Failure(ex.Message, "INSUFFICIENT_FUNDS");
        }
        catch (AccountNotActiveException ex)
        {
            return Result<TransactionDto>.Failure(ex.Message, "ACCOUNT_NOT_ACTIVE");
        }
    }
}
