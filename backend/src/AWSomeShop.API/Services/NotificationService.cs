using AWSomeShop.API.Models.DTOs;
using AWSomeShop.API.Models.Entities;
using AWSomeShop.API.Repositories.Interfaces;
using AWSomeShop.API.Services.Interfaces;

namespace AWSomeShop.API.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        INotificationRepository notificationRepository,
        ILogger<NotificationService> logger)
    {
        _notificationRepository = notificationRepository;
        _logger = logger;
    }

    public async Task CreateNotificationAsync(
        string employeeId,
        string title,
        string content,
        NotificationType type,
        string? relatedId = null)
    {
        var notification = new Notification
        {
            EmployeeId = employeeId,
            Title = title,
            Content = content,
            Type = type,
            RelatedId = relatedId,
            CreatedAt = DateTime.UtcNow
        };

        await _notificationRepository.CreateAsync(notification);
        _logger.LogInformation("Notification created for employee {EmployeeId}: {Title}", employeeId, title);
    }

    public async Task<List<NotificationDto>> GetNotificationsAsync(
        string employeeId,
        int pageNumber = 1,
        int pageSize = 20,
        bool? isRead = null)
    {
        var notifications = await _notificationRepository.GetByEmployeeIdAsync(employeeId, pageNumber, pageSize, isRead);
        return notifications.Select(MapToDto).ToList();
    }

    public async Task<int> GetUnreadCountAsync(string employeeId)
    {
        return await _notificationRepository.GetUnreadCountAsync(employeeId);
    }

    public async Task MarkAsReadAsync(string notificationId, string employeeId)
    {
        var notification = await _notificationRepository.GetByIdAsync(notificationId);
        if (notification == null || notification.EmployeeId != employeeId)
        {
            throw new InvalidOperationException("Notification not found");
        }

        await _notificationRepository.MarkAsReadAsync(notificationId);
        _logger.LogInformation("Notification {NotificationId} marked as read", notificationId);
    }

    public async Task MarkAllAsReadAsync(string employeeId)
    {
        await _notificationRepository.MarkAllAsReadAsync(employeeId);
        _logger.LogInformation("All notifications marked as read for employee {EmployeeId}", employeeId);
    }

    private static NotificationDto MapToDto(Notification notification)
    {
        return new NotificationDto
        {
            Id = notification.Id,
            Title = notification.Title,
            Content = notification.Content,
            Type = notification.Type.ToString(),
            RelatedId = notification.RelatedId,
            IsRead = notification.IsRead,
            CreatedAt = notification.CreatedAt
        };
    }
}
