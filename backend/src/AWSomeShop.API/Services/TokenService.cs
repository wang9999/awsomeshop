using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AWSomeShop.API.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace AWSomeShop.API.Services;

/// <summary>
/// JWT Token generation and validation service
/// </summary>
public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expirationHours;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
        _secretKey = _configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
        _issuer = _configuration["Jwt:Issuer"] ?? "AWSomeShop";
        _audience = _configuration["Jwt:Audience"] ?? "AWSomeShop-Users";
        // 会话超时：30分钟无操作自动退出
        _expirationHours = int.Parse(_configuration["Jwt:ExpirationHours"] ?? "0");
        if (_expirationHours == 0)
        {
            _expirationHours = 0; // 使用分钟配置
        }
    }

    /// <summary>
    /// Generate a JWT token for a user
    /// </summary>
    public string GenerateToken(string userId, string email, string role, Dictionary<string, string>? additionalClaims = null)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
        };

        // Add additional claims if provided
        if (additionalClaims != null)
        {
            foreach (var claim in additionalClaims)
            {
                claims.Add(new Claim(claim.Key, claim.Value));
            }
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // 会话超时：30分钟
        var expirationMinutes = int.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "30");
        
        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Validate a JWT token
    /// </summary>
    public ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            return principal;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Get user ID from token
    /// </summary>
    public string? GetUserIdFromToken(string token)
    {
        var principal = ValidateToken(token);
        return principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}
