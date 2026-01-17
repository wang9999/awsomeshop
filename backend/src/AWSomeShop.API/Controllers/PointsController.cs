using AWSomeShop.API.Models.DTOs;
using AWSomeShop.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AWSomeShop.API.Controllers;

[ApiController]
[Route("api")]
public class PointsController : ControllerBase
{
    private readonly IPointsService _pointsService;
    private readonly ILogger<PointsController> _logger;

    public PointsController(IPointsService pointsService, ILogger<PointsController> logger)
    {
        _pointsService = pointsService;
        _logger = logger;
    }

    /// <summary>
    /// Get current employee's points balance
    /// </summary>
    [HttpGet("points/balance")]
    [Authorize(Roles = "Employee")]
    public async Task<ActionResult<PointsBalanceResponse>> GetBalance()
    {
        try
        {
            var employeeId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(employeeId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var balance = await _pointsService.GetBalanceAsync(employeeId);
            var expiringPoints = await _pointsService.GetExpiringPointsAsync(employeeId);

            return Ok(new PointsBalanceResponse
            {
                Balance = balance,
                ExpiringPoints = expiringPoints
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting points balance");
            return StatusCode(500, new { message = "Failed to get points balance" });
        }
    }

    /// <summary>
    /// Get current employee's points transaction history
    /// </summary>
    [HttpGet("points/transactions")]
    [Authorize(Roles = "Employee")]
    public async Task<ActionResult<PointsTransactionListResponse>> GetTransactions(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        try
        {
            var employeeId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(employeeId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var (transactions, totalCount) = await _pointsService.GetTransactionHistoryAsync(
                employeeId, pageNumber, pageSize, startDate, endDate);

            var response = new PointsTransactionListResponse
            {
                Transactions = transactions.Select(t => new PointsTransactionResponse
                {
                    Id = t.Id,
                    EmployeeId = t.EmployeeId,
                    EmployeeName = t.Employee?.Name ?? "",
                    Amount = t.Amount,
                    Type = t.Type.ToString(),
                    Reason = t.Reason,
                    OperatorId = t.OperatorId,
                    OperatorType = t.OperatorType.ToString(),
                    BalanceBefore = t.BalanceBefore,
                    BalanceAfter = t.BalanceAfter,
                    RelatedOrderId = t.RelatedOrderId,
                    ExpiryDate = t.ExpiryDate,
                    CreatedAt = t.CreatedAt
                }).ToList(),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting transaction history");
            return StatusCode(500, new { message = "Failed to get transaction history" });
        }
    }

    /// <summary>
    /// Export current employee's points transaction history as CSV
    /// </summary>
    [HttpGet("points/transactions/export")]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> ExportTransactions(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        try
        {
            var employeeId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(employeeId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            // Get all transactions without pagination
            var (transactions, _) = await _pointsService.GetTransactionHistoryAsync(
                employeeId, 1, int.MaxValue, startDate, endDate);

            // Generate CSV content
            var csv = new System.Text.StringBuilder();
            csv.AppendLine("交易时间,类型,金额,原因,操作者,余额变化前,余额变化后,过期时间");

            foreach (var t in transactions)
            {
                csv.AppendLine($"{t.CreatedAt:yyyy-MM-dd HH:mm:ss}," +
                    $"{t.Type}," +
                    $"{t.Amount}," +
                    $"\"{t.Reason ?? ""}\"," +
                    $"{t.OperatorType}," +
                    $"{t.BalanceBefore}," +
                    $"{t.BalanceAfter}," +
                    $"{(t.ExpiryDate.HasValue ? t.ExpiryDate.Value.ToString("yyyy-MM-dd") : "")}");
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
            var fileName = $"points_transactions_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";

            return File(bytes, "text/csv", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting transaction history");
            return StatusCode(500, new { message = "Failed to export transaction history" });
        }
    }

    /// <summary>
    /// Admin: Grant points to a single employee
    /// </summary>
    [HttpPost("admin/points/grant")]
    [Authorize(Roles = "SuperAdmin,PointsAdmin")]
    public async Task<ActionResult> GrantPoints([FromBody] GrantPointsRequest request)
    {
        try
        {
            if (request.Amount <= 0)
            {
                return BadRequest(new { message = "Amount must be positive" });
            }

            var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(adminId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            await _pointsService.AddPointsAsync(
                request.EmployeeId,
                request.Amount,
                request.Reason,
                adminId,
                request.ExpiryDate);

            _logger.LogInformation(
                "Admin {AdminId} granted {Amount} points to employee {EmployeeId}",
                adminId, request.Amount, request.EmployeeId);

            return Ok(new { message = "Points granted successfully" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error granting points");
            return StatusCode(500, new { message = "Failed to grant points" });
        }
    }

    /// <summary>
    /// Admin: Grant points to multiple employees (batch operation)
    /// </summary>
    [HttpPost("admin/points/grant/batch")]
    [Authorize(Roles = "SuperAdmin,PointsAdmin")]
    public async Task<ActionResult> BatchGrantPoints([FromBody] BatchGrantPointsRequest request)
    {
        try
        {
            if (request.Amount <= 0)
            {
                return BadRequest(new { message = "Amount must be positive" });
            }

            if (request.EmployeeIds == null || !request.EmployeeIds.Any())
            {
                return BadRequest(new { message = "Employee IDs cannot be empty" });
            }

            var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(adminId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var successCount = 0;
            var failedEmployees = new List<string>();

            // Process each employee - atomic per employee
            foreach (var employeeId in request.EmployeeIds)
            {
                try
                {
                    await _pointsService.AddPointsAsync(
                        employeeId,
                        request.Amount,
                        request.Reason,
                        adminId,
                        request.ExpiryDate);
                    successCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to grant points to employee {EmployeeId}", employeeId);
                    failedEmployees.Add(employeeId);
                }
            }

            _logger.LogInformation(
                "Admin {AdminId} batch granted {Amount} points to {SuccessCount}/{TotalCount} employees",
                adminId, request.Amount, successCount, request.EmployeeIds.Count);

            return Ok(new
            {
                message = $"Points granted to {successCount} out of {request.EmployeeIds.Count} employees",
                successCount,
                totalCount = request.EmployeeIds.Count,
                failedEmployees
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in batch grant points");
            return StatusCode(500, new { message = "Failed to grant points" });
        }
    }

    /// <summary>
    /// Admin: Deduct points from an employee
    /// </summary>
    [HttpPost("admin/points/deduct")]
    [Authorize(Roles = "SuperAdmin,PointsAdmin")]
    public async Task<ActionResult> DeductPoints([FromBody] DeductPointsRequest request)
    {
        try
        {
            if (request.Amount <= 0)
            {
                return BadRequest(new { message = "Amount must be positive" });
            }

            var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(adminId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            await _pointsService.DeductPointsAsync(
                request.EmployeeId,
                request.Amount,
                request.Reason,
                adminId);

            _logger.LogInformation(
                "Admin {AdminId} deducted {Amount} points from employee {EmployeeId}",
                adminId, request.Amount, request.EmployeeId);

            return Ok(new { message = "Points deducted successfully" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deducting points");
            return StatusCode(500, new { message = "Failed to deduct points" });
        }
    }
}
