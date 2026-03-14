using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using TechSalaryIdentity.Core.DTOs;
using TechSalaryIdentity.Core.Entities;
using TechSalaryIdentity.Core.Interfaces;
using TechSalaryIdentity.Infrastructure.Data;

namespace TechSalaryIdentity.API.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly AppDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthenticationService> _logger;

        public AuthenticationService(AppDbContext context, ITokenService tokenService, ILogger<AuthenticationService> logger)
        {
            _context = context;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
        {
            try
            {
                // Validate input
                if (!IsValidEmail(request.Email))
                    return new AuthResponseDto { Success = false, Message = "Invalid email format" };

                if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
                    return new AuthResponseDto { Success = false, Message = "Password must be at least 6 characters" };

                if (request.Password != request.ConfirmPassword)
                    return new AuthResponseDto { Success = false, Message = "Passwords do not match" };

                // Check if user already exists
                var existingUser = _context.Users.FirstOrDefault(u => u.Email == request.Email);
                if (existingUser != null)
                    return new AuthResponseDto { Success = false, Message = "Email already registered" };

                // Create new user
                var user = new User
                {
                    UserId = Guid.NewGuid(),
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    Role = "User", // Default role
                    IsActive = true,
                    IsEmailVerified = false,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"User registered successfully: {user.Email}");

                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Registration successful",
                    User = MapToUserDto(user)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during registration: {ex.Message}");
                return new AuthResponseDto { Success = false, Message = "Registration failed" };
            }
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
        {
            try
            {
                // Find user by email
                var user = _context.Users.FirstOrDefault(u => u.Email == request.Email);
                if (user == null)
                    return new AuthResponseDto { Success = false, Message = "Invalid email or password" };

                // Check if user is active
                if (!user.IsActive)
                    return new AuthResponseDto { Success = false, Message = "User account is inactive" };

                // Verify password
                if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                    return new AuthResponseDto { Success = false, Message = "Invalid email or password" };

                // Generate tokens
                var accessToken = _tokenService.GenerateAccessToken(user);
                var refreshToken = _tokenService.GenerateRefreshToken(user.UserId);

                // Save refresh token to database
                _context.AuthRefreshTokens.Add(refreshToken);
                user.LastLoginAt = DateTime.UtcNow;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"User logged in successfully: {user.Email}");

                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Login successful",
                    AccessToken = accessToken,
                    RefreshToken = refreshToken.Token,
                    User = MapToUserDto(user),
                    ExpiresAt = refreshToken.ExpiresAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during login: {ex.Message}");
                return new AuthResponseDto { Success = false, Message = "Login failed" };
            }
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                // Find refresh token in database
                var storedToken = _context.AuthRefreshTokens
                    .Include(rt => rt.User)
                    .FirstOrDefault(rt => rt.Token == refreshToken && !rt.IsRevoked);

                if (storedToken == null || storedToken.ExpiresAt < DateTime.UtcNow)
                    return new AuthResponseDto { Success = false, Message = "Invalid or expired refresh token" };

                var user = storedToken.User;
                if (user == null || !user.IsActive)
                    return new AuthResponseDto { Success = false, Message = "User not found or inactive" };

                // Generate new access token
                var newAccessToken = _tokenService.GenerateAccessToken(user);

                _logger.LogInformation($"Token refreshed for user: {user.Email}");

                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Token refreshed successfully",
                    AccessToken = newAccessToken,
                    RefreshToken = refreshToken,
                    User = MapToUserDto(user),
                    ExpiresAt = storedToken.ExpiresAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during token refresh: {ex.Message}");
                return new AuthResponseDto { Success = false, Message = "Token refresh failed" };
            }
        }

        public async Task<bool> RevokeTokenAsync(string refreshToken)
        {
            try
            {
                var token = _context.AuthRefreshTokens.FirstOrDefault(rt => rt.Token == refreshToken);
                if (token == null)
                    return false;

                token.IsRevoked = true;
                _context.AuthRefreshTokens.Update(token);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Refresh token revoked");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error revoking token: {ex.Message}");
                return false;
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private UserDto MapToUserDto(User user)
        {
            return new UserDto
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role,
                IsActive = user.IsActive
            };
        }
    }
}
