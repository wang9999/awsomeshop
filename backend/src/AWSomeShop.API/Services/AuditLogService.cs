using AWSomeShop.API.Models.Entities;
using AWSomeShop.API.Repositories.Interfaces;
using AWSomeShop.API.Services.Interfaces;
using System.Text.Json;

namespace AWSomeShop.API.Services;

public class AuditLogService : IAuditLogService
{
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly ILogger<AuditLogService> _logger;

    public AuditLogService(
        IAuditLogRepository auditLogRepository,
        ILogger<AuditLogService> logger)
    {
        _auditLogRepository = auditLogRepository;
        _logger = logger;
    }

    public async Task LogAsync(
        string adminId,
        string adminName,
        string action,
        string entityType,
        string? entityId = null,
        object? oldValue = null,
        object? newValue = null,
        string? ipAddress = null,
        string? userAgent = null)
    {
        var auditLog = new AuditLog
        {
            AdminId = adminId,
            AdminName = adminName,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            OldValue = oldValue != null ? JsonSerializer.Serialize(oldValue) : null,
            NewValue = newValue != null ? JsonSerializer.Serialize(newValue) : null,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            CreatedAt = DateTime.UtcNow
        };

        await _auditLogRepository.CreateAsync(auditLog);
        _logger.LogInformation("Audit log created: {Action} on {EntityType} by {AdminName}", action, entityType, adminName);
    }

    public async Task<(List<AuditLog> Logs, int TotalCount)> GetLogsAsync(
        int pageNumber = 1,
        int pageSize = 20,
        string? adminId = null,
        string? action = null,
        string? entityType = null,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        return await _auditLogRepository.GetLogsAsync(
            pageNumber, pageSize, adminId, action, entityType, startDate, endDate);
    }

    public async Task<List<AuditLog>> GetLogsByEntityAsync(string entityType, string entityId)
    {
        return await _auditLogRepository.GetLogsByEntityAsync(entityType, entityId);
    }
}
