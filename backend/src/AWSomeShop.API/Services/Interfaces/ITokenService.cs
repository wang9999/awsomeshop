using System.Security.Claims;

namespace AWSomeShop.API.Services.Interfaces;

/// <summary>
/// JWT Token generation and validation service
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generate a JWT token for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="email">User email or username</param>
    /// <param name="role">User role</param>
    /// <param name="additionalClaims">Additional claims to include in the token</param>
    /// <returns>JWT token string</returns>
    string GenerateToken(string userId, string email, string role, Dictionary<string, string>? additionalClaims = null);

    /// <summary>
    /// Validate a JWT token
    /// </summary>
    /// <param name="token">JWT token to validate</param>
    /// <returns>ClaimsPrincipal if valid, null otherwise</returns>
    ClaimsPrincipal? ValidateToken(string token);

    /// <summary>
    /// Get user ID from token
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>User ID if valid, null otherwise</returns>
    string? GetUserIdFromToken(string token);
}
