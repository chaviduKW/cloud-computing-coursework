using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IdentityApi.Core.DTOs;
using IdentityApi.Core.Interfaces;

namespace IdentityApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthenticationService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="request">Registration request with email, password, and name</param>
    /// <returns>Registration response with user details</returns>
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new AuthResponseDto { Success = false, Message = "Invalid input" });

        var result = await _authService.RegisterAsync(request);
        
        if (!result.Success)
            return BadRequest(result);

        return StatusCode(201, result);
    }

    /// <summary>
    /// Login user with email and password
    /// </summary>
    /// <param name="request">Login request with email and password</param>
    /// <returns>Login response with access token and refresh token</returns>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new AuthResponseDto { Success = false, Message = "Invalid input" });

        var result = await _authService.LoginAsync(request);
        
        if (!result.Success)
            return Unauthorized(result);

        return Ok(result);
    }

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    /// <param name="request">Refresh token request</param>
    /// <returns>New access token</returns>
    [HttpPost("refresh-token")]
    public async Task<ActionResult<AuthResponseDto>> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            return BadRequest(new AuthResponseDto { Success = false, Message = "Refresh token is required" });

        var result = await _authService.RefreshTokenAsync(request.RefreshToken);
        
        if (!result.Success)
            return Unauthorized(result);

        return Ok(result);
    }

    /// <summary>
    /// Logout user by revoking refresh token
    /// </summary>
    /// <param name="request">Refresh token to revoke</param>
    /// <returns>Logout response</returns>
    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult<AuthResponseDto>> Logout([FromBody] RefreshTokenRequestDto request)
    {
        var isRevoked = await _authService.RevokeTokenAsync(request.RefreshToken);
        
        if (!isRevoked)
            return BadRequest(new AuthResponseDto { Success = false, Message = "Failed to logout" });

        return Ok(new AuthResponseDto { Success = true, Message = "Logged out successfully" });
    }

    /// <summary>
    /// Get current user profile (requires authentication)
    /// </summary>
    /// <returns>Current user information</returns>
    [HttpGet("me")]
    [Authorize]
    public ActionResult<UserDto> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var emailClaim = User.FindFirst(ClaimTypes.Email)?.Value;
        var firstNameClaim = User.FindFirst(ClaimTypes.GivenName)?.Value;
        var lastNameClaim = User.FindFirst(ClaimTypes.Surname)?.Value;
        var fullNameClaim = User.FindFirst(ClaimTypes.Name)?.Value;
        var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
        var isActiveClaim = User.FindFirst("IsActive")?.Value;

        if ((string.IsNullOrWhiteSpace(firstNameClaim) || string.IsNullOrWhiteSpace(lastNameClaim))
            && !string.IsNullOrWhiteSpace(fullNameClaim))
        {
            var nameParts = fullNameClaim.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            firstNameClaim ??= nameParts.Length > 0 ? nameParts[0] : string.Empty;
            lastNameClaim ??= nameParts.Length > 1 ? nameParts[1] : string.Empty;
        }

        var user = new UserDto
        {
            UserId = Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty,
            FirstName = firstNameClaim ?? string.Empty,
            LastName = lastNameClaim ?? string.Empty,
            Email = emailClaim ?? string.Empty,
            Role = roleClaim ?? string.Empty,
            IsActive = bool.TryParse(isActiveClaim, out var isActive) && isActive
        };

        return Ok(user);
    }

    /// <summary>
    /// Admin endpoint - Get all users (Admin only)
    /// </summary>
    /// <returns>List of all users</returns>
    [HttpGet("users")]
    [Authorize(Roles = "Admin")]
    public ActionResult<IEnumerable<UserDto>> GetAllUsers()
    {
        return Ok(new { message = "This endpoint requires Admin role" });
    }

    /// <summary>
    /// Check if user has specific role (for authorization verification)
    /// </summary>
    /// <returns>User role information</returns>
    [HttpGet("check-role")]
    [Authorize]
    public ActionResult<object> CheckRole()
    {
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        return Ok(new { role = role ?? "User" });
    }
}
