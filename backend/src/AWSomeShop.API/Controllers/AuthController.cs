using AWSomeShop.API.Models.DTOs;
using AWSomeShop.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AWSomeShop.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Employee login
    /// </summary>
    [HttpPost("employee/login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> EmployeeLogin([FromBody] EmployeeLoginRequest request)
    {
        try
        {
            var response = await _authService.EmployeeLoginAsync(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during employee login");
            return StatusCode(500, new { message = "登录失败，请稍后重试" });
        }
    }

    /// <summary>
    /// Admin login
    /// </summary>
    [HttpPost("admin/login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AdminLogin([FromBody] AdminLoginRequest request)
    {
        try
        {
            var response = await _authService.AdminLoginAsync(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during admin login");
            return StatusCode(500, new { message = "登录失败，请稍后重试" });
        }
    }

    /// <summary>
    /// Get current employee info
    /// </summary>
    [HttpGet("employees/me")]
    [Authorize(Roles = "Employee")]
    [ProducesResponseType(typeof(EmployeeInfoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCurrentEmployee()
    {
        try
        {
            var employeeId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(employeeId))
            {
                return Unauthorized(new { message = "未找到用户信息" });
            }

            var employee = await _authService.GetCurrentEmployeeAsync(employeeId);
            if (employee == null)
            {
                return NotFound(new { message = "员工信息不存在" });
            }

            return Ok(employee);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current employee");
            return StatusCode(500, new { message = "获取员工信息失败" });
        }
    }
}
