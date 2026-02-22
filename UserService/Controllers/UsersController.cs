using Microsoft.AspNetCore.Mvc;
using Shared.Common.Results;
using UserService.Contracts.Requests;
using UserService.Contracts.Responses;
using UserService.Domain.Exceptions;
using UserService.Domain.Services;

namespace UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Get all users
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<UserResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllUsers(CancellationToken ct)
    {
        try
        {
            var response = await _userService.GetAllUsersAsync(ct);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all users");
            return StatusCode(500, Error.Internal("An error occurred while retrieving users"));
        }
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserById(Guid id, CancellationToken ct)
    {
        try
        {
            var response = await _userService.GetUserByIdAsync(id, ct);
            return Ok(response);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(Error.NotFound("User", id));
        }
        catch (UserNotFoundException ex)
        {
            return NotFound(new Error(ex.Code, ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user {UserId}", id);
            return StatusCode(500, Error.Internal("An error occurred while retrieving the user"));
        }
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request, CancellationToken ct)
    {
        try
        {
            var response = await _userService.CreateUserAsync(request, ct);

            return CreatedAtAction(
                nameof(GetUserById),
                new { id = response.Id },
                response
            );
        }
        catch (DomainException ex) when (ex.Message.Contains("email", StringComparison.OrdinalIgnoreCase))
        {
            return Conflict(new Error(ex.Code, ex.Message));
        }
        catch (DomainException ex)
        {
            return BadRequest(new Error(ex.Code, ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return StatusCode(500, Error.Internal("An error occurred while creating the user"));
        }
    }

    /// <summary>
    /// Update an existing user
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest request, CancellationToken ct)
    {
        try
        {
            var response = await _userService.UpdateUserAsync(id, request, ct);
            return Ok(response);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(Error.NotFound("User", id));
        }
        catch (UserNotFoundException ex)
        {
            return NotFound(new Error(ex.Code, ex.Message));
        }
        catch (DomainException ex)
        {
            return BadRequest(new Error(ex.Code, ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {UserId}", id);
            return StatusCode(500, Error.Internal("An error occurred while updating the user"));
        }
    }

    /// <summary>
    /// Delete a user (soft delete)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(Guid id, CancellationToken ct)
    {
        try
        {
            await _userService.DeleteUserAsync(id, ct);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(Error.NotFound("User", id));
        }
        catch (UserNotFoundException ex)
        {
            return NotFound(new Error(ex.Code, ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {UserId}", id);
            return StatusCode(500, Error.Internal("An error occurred while deleting the user"));
        }
    }
}
