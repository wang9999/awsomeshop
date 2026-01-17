namespace AWSomeShop.API.Models.Entities;

public enum NotificationType
{
    积分变动,
    订单状态,
    产品上新,
    系统通知
}

public class Notification
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string EmployeeId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public string? RelatedId { get; set; }
    public bool IsRead { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public Employee Employee { get; set; } = null!;
}
