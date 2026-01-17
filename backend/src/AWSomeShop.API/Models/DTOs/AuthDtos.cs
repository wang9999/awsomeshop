namespace AWSomeShop.API.Models.DTOs;

/// <summary>
/// Employee login request
/// </summary>
public class EmployeeLoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// Admin login request
/// </summary>
public class AdminLoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// Login response
/// </summary>
public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Role { get; set; }
    public int? Points { get; set; }
}

/// <summary>
/// Employee info response
/// </summary>
public class EmployeeInfoResponse
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string EmployeeNumber { get; set; } = string.Empty;
    public string? Department { get; set; }
    public int PointsBalance { get; set; }
    public int MonthlyRedeemCount { get; set; }
}
