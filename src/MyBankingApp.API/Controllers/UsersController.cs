using Microsoft.AspNetCore.Mvc;
using MyBankingApp.Domain.Entities;
using MyBankingApp.Domain.Interfaces;

namespace MyBankingApp.API.Controllers;

/// <summary>
/// API controller for user operations (demo purposes).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public UsersController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Create a demo user.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        // Check if user already exists
        if (await _unitOfWork.Users.ExistsAsync(request.Email, cancellationToken))
        {
            return BadRequest(new { Error = "User with this email already exists", ErrorCode = "USER_EXISTS" });
        }

        var user = new User(request.Email, "demo-password-hash", request.FirstName, request.LastName);
        await _unitOfWork.Users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, new
        {
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.CreatedAt
        });
    }

    /// <summary>
    /// Get user by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUser(Guid id, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetWithAccountsAsync(id, cancellationToken);
        if (user == null)
        {
            return NotFound(new { Error = "User not found", ErrorCode = "USER_NOT_FOUND" });
        }

        return Ok(new
        {
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            Accounts = user.Accounts.Select(a => new
            {
                a.Id,
                a.AccountNumber,
                Type = a.Type.ToString(),
                Status = a.Status.ToString(),
                Balance = a.Balance.Amount,
                a.Balance.Currency
            })
        });
    }

    /// <summary>
    /// Get user by email (for login).
    /// </summary>
    [HttpGet("by-email/{email}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserByEmail(string email, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(email, cancellationToken);
        if (user == null)
        {
            return NotFound(new { Error = "User not found", ErrorCode = "USER_NOT_FOUND" });
        }

        return Ok(new
        {
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName
        });
    }
}

public record CreateUserRequest(string Email, string FirstName, string LastName);
