using MediatR;
using MyBankingApp.Application.Common;
using MyBankingApp.Application.DTOs;

namespace MyBankingApp.Application.Queries.GetAccount;

/// <summary>
/// Query to get account details.
/// </summary>
public record GetAccountQuery(Guid AccountId) : IRequest<Result<AccountDto>>;
