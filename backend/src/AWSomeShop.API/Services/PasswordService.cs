using AWSomeShop.API.Services.Interfaces;

namespace AWSomeShop.API.Services;

/// <summary>
/// Password hashing and verification service using BCrypt
/// </summary>
public class PasswordService : IPasswordService
{
    /// <summary>
    /// Hash a plain text password using BCrypt
    /// </summary>
    /// <param name="password">Plain text password</param>
    /// <returns>Hashed password</returns>
    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Password cannot be null or empty", nameof(password));
        }

        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    /// <summary>
    /// Verify a password against a BCrypt hash
    /// </summary>
    /// <param name="password">Plain text password</param>
    /// <param name="hashedPassword">Hashed password to verify against</param>
    /// <returns>True if password matches, false otherwise</returns>
    public bool VerifyPassword(string password, string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(hashedPassword))
        {
            return false;
        }

        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
        catch
        {
            return false;
        }
    }
}
