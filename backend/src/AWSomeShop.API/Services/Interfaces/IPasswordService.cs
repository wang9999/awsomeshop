namespace AWSomeShop.API.Services.Interfaces;

/// <summary>
/// Password hashing and verification service
/// </summary>
public interface IPasswordService
{
    /// <summary>
    /// Hash a plain text password
    /// </summary>
    /// <param name="password">Plain text password</param>
    /// <returns>Hashed password</returns>
    string HashPassword(string password);

    /// <summary>
    /// Verify a password against a hash
    /// </summary>
    /// <param name="password">Plain text password</param>
    /// <param name="hashedPassword">Hashed password to verify against</param>
    /// <returns>True if password matches, false otherwise</returns>
    bool VerifyPassword(string password, string hashedPassword);
}
