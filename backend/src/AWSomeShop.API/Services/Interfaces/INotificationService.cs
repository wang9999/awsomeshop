using AWSomeShop.API.Models.DTOs;
using AWSomeShop.API.Models.Entities;

namespace AWSomeShop.API.Services.Interfaces;

public interface INotificationService
{
    Task CreateNotificationAsync(string employeeId, string title, string content, NotificationType type, string? relatedId = null);
    Task<List<NotificationDto>> GetNotificationsAsync(string employeeId, int pageNumber = 1, int pageSize = 20, bool? isRead = null);
    Task<int> GetUnreadCountAsync(string employeeId);
    Task MarkAsReadAsync(string notificationId, string employeeId);
    Task MarkAllAsReadAsync(string employeeId);
}
