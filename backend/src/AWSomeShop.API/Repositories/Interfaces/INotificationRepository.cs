using AWSomeShop.API.Models.Entities;

namespace AWSomeShop.API.Repositories.Interfaces;

public interface INotificationRepository
{
    Task<Notification> CreateAsync(Notification notification);
    Task<List<Notification>> GetByEmployeeIdAsync(string employeeId, int pageNumber = 1, int pageSize = 20, bool? isRead = null);
    Task<int> GetUnreadCountAsync(string employeeId);
    Task<Notification?> GetByIdAsync(string id);
    Task<Notification> UpdateAsync(Notification notification);
    Task MarkAsReadAsync(string id);
    Task MarkAllAsReadAsync(string employeeId);
}
