using MediatR;
using MyBankingApp.Application.Common;
using MyBankingApp.Application.DTOs;
using MyBankingApp.Domain.Interfaces;

namespace MyBankingApp.Application.Queries.GetAccount;

/// <summary>
/// Handler for GetAccountQuery.
/// </summary>
public class GetAccountQueryHandler : IRequestHandler<GetAccountQuery, Result<AccountDto>>
{
    private readonly IAccountRepository _accountRepository;

    public GetAccountQueryHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<Result<AccountDto>> Handle(GetAccountQuery request, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetByIdAsync(request.AccountId, cancellationToken);
        if (account == null)
        {
            return Result<AccountDto>.Failure("Account not found", "ACCOUNT_NOT_FOUND");
        }

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
