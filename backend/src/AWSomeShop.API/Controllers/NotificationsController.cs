using AWSomeShop.API.Models.DTOs;
using AWSomeShop.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AWSomeShop.API.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize(Roles = "Employee")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(
        INotificationService notificationService,
        ILogger<NotificationsController> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    /// <summary>
    /// Get notifications for current employee
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<object>> GetNotifications(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool? isRead = null)
    {
        try
        {
            var employeeId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(employeeId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var notifications = await _notificationService.GetNotificationsAsync(employeeId, page, pageSize, isRead);
            var unreadCount = await _notificationService.GetUnreadCountAsync(employeeId);

            return Ok(new
            {
                notifications,
                unreadCount,
                page,
                pageSize
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notifications");
            return StatusCode(500, new { message = "Failed to get notifications" });
        }
    }

    /// <summary>
    /// Get unread notification count
    /// </summary>
    [HttpGet("unread-count")]
    public async Task<ActionResult<object>> GetUnreadCount()
    {
        try
        {
            var employeeId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(employeeId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var count = await _notificationService.GetUnreadCountAsync(employeeId);
            return Ok(new { count });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting unread count");
            return StatusCode(500, new { message = "Failed to get unread count" });
        }
    }

    /// <summary>
    /// Mark notification as read
    /// </summary>
    [HttpPut("{id}/read")]
    public async Task<ActionResult> MarkAsRead(string id)
    {
        try
        {
            var employeeId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(employeeId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            await _notificationService.MarkAsReadAsync(id, employeeId);
            return Ok(new { message = "Notification marked as read" });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking notification as read");
            return StatusCode(500, new { message = "Failed to mark notification as read" });
        }
    }

    /// <summary>
    /// Mark all notifications as read
    /// </summary>
    [HttpPut("read-all")]
    public async Task<ActionResult> MarkAllAsRead()
    {
        try
        {
            var employeeId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(employeeId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            await _notificationService.MarkAllAsReadAsync(employeeId);
            return Ok(new { message = "All notifications marked as read" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking all notifications as read");
            return StatusCode(500, new { message = "Failed to mark all notifications as read" });
        }
    }
}
