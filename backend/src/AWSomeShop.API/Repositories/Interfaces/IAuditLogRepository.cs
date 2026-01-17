using AWSomeShop.API.Models.Entities;

namespace AWSomeShop.API.Repositories.Interfaces;

public interface IAuditLogRepository
{
    Task<AuditLog> CreateAsync(AuditLog auditLog);
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
