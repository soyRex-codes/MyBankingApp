using MediatR;
using MyBankingApp.Application.Common;
using MyBankingApp.Application.DTOs;
using MyBankingApp.Domain.Entities;
using MyBankingApp.Domain.Interfaces;

namespace MyBankingApp.Application.Commands.CreateAccount;

/// <summary>
/// Handler for CreateAccountCommand.
/// </summary>
public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, Result<AccountDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateAccountCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AccountDto>> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        // Verify user exists
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            return Result<AccountDto>.Failure("User not found", "USER_NOT_FOUND");
        }

        // Create the account
        var account = new Account(request.UserId, request.Type, request.Currency);
        
        await _unitOfWork.Accounts.AddAsync(account, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = new AccountDto(
            account.Id,
            account.AccountNumber,
            account.Type.ToString(),
            account.Status.ToString(),
            account.Balance.Amount,
            account.Balance.Currency,
            account.CreatedAt);

        return Result<AccountDto>.Success(dto);
    }
}
