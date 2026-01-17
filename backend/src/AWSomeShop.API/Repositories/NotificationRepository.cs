using AWSomeShop.API.Infrastructure.DbContext;
using AWSomeShop.API.Models.Entities;
using AWSomeShop.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AWSomeShop.API.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly AWSomeShopDbContext _context;

    public NotificationRepository(AWSomeShopDbContext context)
    {
        _context = context;
    }

    public async Task<Notification> CreateAsync(Notification notification)
    {
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
        return notification;
    }

    public async Task<List<Notification>> GetByEmployeeIdAsync(string employeeId, int pageNumber = 1, int pageSize = 20, bool? isRead = null)
    {
        var query = _context.Notifications
            .Where(n => n.EmployeeId == employeeId);

        if (isRead.HasValue)
        {
            query = query.Where(n => n.IsRead == isRead.Value);
        }

        return await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetUnreadCountAsync(string employeeId)
    {
        return await _context.Notifications
            .Where(n => n.EmployeeId == employeeId && !n.IsRead)
            .CountAsync();
    }

    public async Task<Notification?> GetByIdAsync(string id)
    {
        return await _context.Notifications.FindAsync(id);
    }

    public async Task<Notification> UpdateAsync(Notification notification)
    {
        _context.Notifications.Update(notification);
        await _context.SaveChangesAsync();
        return notification;
    }

    public async Task MarkAsReadAsync(string id)
    {
        var notification = await _context.Notifications.FindAsync(id);
        if (notification != null)
        {
            notification.IsRead = true;
            await _context.SaveChangesAsync();
        }
    }

    public async Task MarkAllAsReadAsync(string employeeId)
    {
        var notifications = await _context.Notifications
            .Where(n => n.EmployeeId == employeeId && !n.IsRead)
            .ToListAsync();

        foreach (var notification in notifications)
        {
            notification.IsRead = true;
        }

        await _context.SaveChangesAsync();
    }
}
