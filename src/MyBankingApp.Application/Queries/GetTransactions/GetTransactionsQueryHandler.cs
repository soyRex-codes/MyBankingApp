using MediatR;
using MyBankingApp.Application.Common;
using MyBankingApp.Application.DTOs;
using MyBankingApp.Domain.Interfaces;

namespace MyBankingApp.Application.Queries.GetTransactions;

/// <summary>
/// Handler for GetTransactionsQuery.
/// </summary>
public class GetTransactionsQueryHandler : IRequestHandler<GetTransactionsQuery, Result<PagedResult<TransactionDto>>>
{
    private readonly IAccountRepository _accountRepository;
    private readonly ITransactionRepository _transactionRepository;

    public GetTransactionsQueryHandler(
        IAccountRepository accountRepository,
        ITransactionRepository transactionRepository)
    {
        _accountRepository = accountRepository;
        _transactionRepository = transactionRepository;
    }

    public async Task<Result<PagedResult<TransactionDto>>> Handle(GetTransactionsQuery request, CancellationToken cancellationToken)
    {
        // Verify account exists
        var account = await _accountRepository.GetByIdAsync(request.AccountId, cancellationToken);
        if (account == null)
        {
            return Result<PagedResult<TransactionDto>>.Failure("Account not found", "ACCOUNT_NOT_FOUND");
        }

        var transactions = await _transactionRepository.GetByAccountIdAsync(
            request.AccountId,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var totalCount = await _transactionRepository.GetCountByAccountIdAsync(request.AccountId, cancellationToken);

        var dtos = transactions.Select(t => new TransactionDto(
            t.Id,
            t.ReferenceNumber,
            t.Type.ToString(),
            t.Status.ToString(),
            t.Amount.Amount,
            t.Amount.Currency,
            t.BalanceAfter.Amount,
            t.Description,
            t.CreatedAt));

        var pagedResult = new PagedResult<TransactionDto>(
            dtos,
            totalCount,
            request.PageNumber,
            request.PageSize);

        return Result<PagedResult<TransactionDto>>.Success(pagedResult);
    }
}
