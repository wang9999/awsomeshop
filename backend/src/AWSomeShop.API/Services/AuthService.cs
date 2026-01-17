using AWSomeShop.API.Infrastructure.DbContext;
using AWSomeShop.API.Models.DTOs;
using AWSomeShop.API.Repositories.Interfaces;
using AWSomeShop.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AWSomeShop.API.Services;

public class AuthService : IAuthService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly AWSomeShopDbContext _context;
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;

    public AuthService(
        IEmployeeRepository employeeRepository,
        AWSomeShopDbContext context,
        IPasswordService passwordService,
        ITokenService tokenService)
    {
        _employeeRepository = employeeRepository;
        _context = context;
        _passwordService = passwordService;
        _tokenService = tokenService;
    }

    public async Task<LoginResponse> EmployeeLoginAsync(EmployeeLoginRequest request)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            throw new UnauthorizedAccessException("邮箱和密码不能为空");
        }

        // Find employee by email
        var employee = await _employeeRepository.GetByEmailAsync(request.Email);
        if (employee == null)
        {
            throw new UnauthorizedAccessException("邮箱或密码错误");
        }

        // Verify password
        if (!_passwordService.VerifyPassword(request.Password, employee.PasswordHash))
        {
            throw new UnauthorizedAccessException("邮箱或密码错误");
        }

        // Generate token
        var token = _tokenService.GenerateToken(
            employee.Id,
            employee.Email,
            "Employee",
            new Dictionary<string, string>
            {
                { "name", employee.Name },
                { "employeeNumber", employee.EmployeeNumber }
            }
        );

        return new LoginResponse
        {
            Token = token,
            Id = employee.Id,
            Name = employee.Name,
            Email = employee.Email,
            Role = "Employee",
            Points = employee.PointsBalance
        };
    }

    public async Task<LoginResponse> AdminLoginAsync(AdminLoginRequest request)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            throw new UnauthorizedAccessException("用户名和密码不能为空");
        }

        // Find admin by username
        var admin = await _context.Administrators
            .FirstOrDefaultAsync(a => a.Username == request.Username && a.IsActive);

        if (admin == null)
        {
            throw new UnauthorizedAccessException("用户名或密码错误");
        }

        // Verify password
        if (!_passwordService.VerifyPassword(request.Password, admin.PasswordHash))
        {
            throw new UnauthorizedAccessException("用户名或密码错误");
        }

        // Generate token
        var token = _tokenService.GenerateToken(
            admin.Id,
            admin.Username,
            admin.Role.ToString(),
            new Dictionary<string, string>
            {
                { "name", admin.Name }
            }
        );

        return new LoginResponse
        {
            Token = token,
            Id = admin.Id,
            Name = admin.Name,
            Role = admin.Role.ToString()
        };
    }

    public async Task<EmployeeInfoResponse?> GetCurrentEmployeeAsync(string employeeId)
    {
        var employee = await _employeeRepository.GetByIdAsync(employeeId);
        if (employee == null)
        {
            return null;
        }

        return new EmployeeInfoResponse
        {
            Id = employee.Id,
            Email = employee.Email,
            Name = employee.Name,
            EmployeeNumber = employee.EmployeeNumber,
            Department = employee.Department,
            PointsBalance = employee.PointsBalance,
            MonthlyRedeemCount = employee.MonthlyRedeemCount
        };
    }
}
