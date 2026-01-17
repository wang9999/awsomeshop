using AWSomeShop.API.Models.Entities;

namespace AWSomeShop.API.Services.Interfaces;

public interface IAuditLogService
{
    Task LogAsync(
        string adminId,
        string adminName,
        string action,
        string entityType,
        string? entityId = null,
        object? oldValue = null,
        object? newValue = null,
        string? ipAddress = null,
        string? userAgent = null);

    Task<(List<AuditLog> Logs, int TotalCount)> GetLogsAsync(
        int pageNumber = 1,
        int pageSize = 20,
        string? adminId = null,
        string? action = null,
        string? entityType = null,
        DateTime? startDate = null,
        DateTime? endDate = null);

    Task<List<AuditLog>> GetLogsByEntityAsync(string entityType, string entityId);
}
