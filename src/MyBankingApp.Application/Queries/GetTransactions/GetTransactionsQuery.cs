using MediatR;
using MyBankingApp.Application.Common;
using MyBankingApp.Application.DTOs;

namespace MyBankingApp.Application.Queries.GetTransactions;

/// <summary>
/// Query to get transaction history for an account.
/// </summary>
public record GetTransactionsQuery(
    Guid AccountId,
    int PageNumber = 1,
    int PageSize = 20) : IRequest<Result<PagedResult<TransactionDto>>>;

/// <summary>
/// Paged result wrapper for list queries.
/// </summary>
public record PagedResult<T>(
    IEnumerable<T> Items,
    int TotalCount,
    int PageNumber,
    int PageSize)
{
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => PageNumber < TotalPages;
    public bool HasPreviousPage => PageNumber > 1;
}
