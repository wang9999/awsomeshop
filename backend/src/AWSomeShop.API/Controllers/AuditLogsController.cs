using AWSomeShop.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AWSomeShop.API.Controllers;

[ApiController]
[Route("api/admin/audit-logs")]
[Authorize(Roles = "SuperAdmin")]
public class AuditLogsController : ControllerBase
{
    private readonly IAuditLogService _auditLogService;
    private readonly ILogger<AuditLogsController> _logger;

    public AuditLogsController(
        IAuditLogService auditLogService,
        ILogger<AuditLogsController> logger)
    {
        _auditLogService = auditLogService;
        _logger = logger;
    }

    /// <summary>
    /// Get audit logs with filters
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<object>> GetLogs(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? adminId = null,
        [FromQuery] string? action = null,
        [FromQuery] string? entityType = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        try
        {
            var (logs, totalCount) = await _auditLogService.GetLogsAsync(
                page, pageSize, adminId, action, entityType, startDate, endDate);

            return Ok(new
            {
                logs,
                totalCount,
                page,
                pageSize,
                totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting audit logs");
            return StatusCode(500, new { message = "Failed to get audit logs" });
        }
    }

    /// <summary>
    /// Get audit logs for specific entity
    /// </summary>
    [HttpGet("entity/{entityType}/{entityId}")]
    public async Task<ActionResult<object>> GetLogsByEntity(string entityType, string entityId)
    {
        try
        {
            var logs = await _auditLogService.GetLogsByEntityAsync(entityType, entityId);
            return Ok(new { logs });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting audit logs for entity");
            return StatusCode(500, new { message = "Failed to get audit logs" });
        }
    }
}
