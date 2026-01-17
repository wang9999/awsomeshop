using AWSomeShop.API.Infrastructure.DbContext;
using AWSomeShop.API.Models.Entities;
using AWSomeShop.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AWSomeShop.API.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly AWSomeShopDbContext _context;

    public AuditLogRepository(AWSomeShopDbContext context)
    {
        _context = context;
    }

    public async Task<AuditLog> CreateAsync(AuditLog auditLog)
    {
        _context.AuditLogs.Add(auditLog);
        await _context.SaveChangesAsync();
        return auditLog;
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
        var query = _context.AuditLogs.AsQueryable();

        if (!string.IsNullOrEmpty(adminId))
        {
            query = query.Where(log => log.AdminId == adminId);
        }

        if (!string.IsNullOrEmpty(action))
        {
            query = query.Where(log => log.Action.Contains(action));
        }

        if (!string.IsNullOrEmpty(entityType))
        {
            query = query.Where(log => log.EntityType == entityType);
        }

        if (startDate.HasValue)
        {
            query = query.Where(log => log.CreatedAt >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(log => log.CreatedAt <= endDate.Value);
        }

        var totalCount = await query.CountAsync();

        var logs = await query
            .OrderByDescending(log => log.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (logs, totalCount);
    }

    public async Task<List<AuditLog>> GetLogsByEntityAsync(string entityType, string entityId)
    {
        return await _context.AuditLogs
            .Where(log => log.EntityType == entityType && log.EntityId == entityId)
            .OrderByDescending(log => log.CreatedAt)
            .ToListAsync();
    }
}
