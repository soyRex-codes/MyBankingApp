using MediatR;
using Microsoft.AspNetCore.Mvc;
using MyBankingApp.Application.Commands.Transfer;

namespace MyBankingApp.API.Controllers;

/// <summary>
/// API controller for transaction operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TransactionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TransactionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Transfer funds between accounts.
    /// </summary>
    [HttpPost("transfer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Transfer(
        [FromBody] TransferRequest request,
        CancellationToken cancellationToken)
    {
        var command = new TransferCommand(
            request.SourceAccountId,
            request.DestinationAccountId,
            request.Amount,
            request.Currency,
            request.Description);

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.ErrorCode switch
            {
                "SOURCE_ACCOUNT_NOT_FOUND" => NotFound(new { result.Error, result.ErrorCode }),
                "DEST_ACCOUNT_NOT_FOUND" => NotFound(new { result.Error, result.ErrorCode }),
                "INSUFFICIENT_FUNDS" => BadRequest(new { result.Error, result.ErrorCode }),
                _ => BadRequest(new { result.Error, result.ErrorCode })
            };
        }

        return Ok(result.Data);
    }
}

public record TransferRequest(
    Guid SourceAccountId,
    Guid DestinationAccountId,
    decimal Amount,
    string Currency = "USD",
    string? Description = null);
