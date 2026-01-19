using MediatR;
using Microsoft.AspNetCore.Mvc;
using MyBankingApp.Application.Commands.CreateAccount;
using MyBankingApp.Application.Commands.Deposit;
using MyBankingApp.Application.Commands.Withdraw;
using MyBankingApp.Application.Commands.Transfer;
using MyBankingApp.Application.Queries.GetAccount;
using MyBankingApp.Application.Queries.GetTransactions;
using MyBankingApp.Domain.Enums;

namespace MyBankingApp.API.Controllers;

/// <summary>
/// API controller for bank account operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AccountsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AccountsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get account details by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAccount(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAccountQuery(id), cancellationToken);
        
        if (!result.IsSuccess)
            return NotFound(new { result.Error, result.ErrorCode });
        
        return Ok(result.Data);
    }

    /// <summary>
    /// Create a new bank account.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAccount(
        [FromBody] CreateAccountRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateAccountCommand(request.UserId, request.Type, request.Currency);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { result.Error, result.ErrorCode });

        return CreatedAtAction(nameof(GetAccount), new { id = result.Data!.Id }, result.Data);
    }

    /// <summary>
    /// Deposit funds into an account.
    /// </summary>
    [HttpPost("{id:guid}/deposit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deposit(
        Guid id,
        [FromBody] DepositRequest request,
        CancellationToken cancellationToken)
    {
        var command = new DepositCommand(id, request.Amount, request.Currency, request.Description);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.ErrorCode == "ACCOUNT_NOT_FOUND" 
                ? NotFound(new { result.Error, result.ErrorCode })
                : BadRequest(new { result.Error, result.ErrorCode });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Withdraw funds from an account.
    /// </summary>
    [HttpPost("{id:guid}/withdraw")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Withdraw(
        Guid id,
        [FromBody] WithdrawRequest request,
        CancellationToken cancellationToken)
    {
        var command = new WithdrawCommand(id, request.Amount, request.Currency, request.Description);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.ErrorCode switch
            {
                "ACCOUNT_NOT_FOUND" => NotFound(new { result.Error, result.ErrorCode }),
                "INSUFFICIENT_FUNDS" => BadRequest(new { result.Error, result.ErrorCode }),
                _ => BadRequest(new { result.Error, result.ErrorCode })
            };
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Get transaction history for an account.
    /// </summary>
    [HttpGet("{id:guid}/transactions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTransactions(
        Guid id,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetTransactionsQuery(id, pageNumber, pageSize);
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return NotFound(new { result.Error, result.ErrorCode });

        return Ok(result.Data);
    }
}

// Request DTOs
public record CreateAccountRequest(Guid UserId, AccountType Type, string Currency = "USD");
public record DepositRequest(decimal Amount, string Currency = "USD", string Description = "Deposit");
public record WithdrawRequest(decimal Amount, string Currency = "USD", string Description = "Withdrawal");
