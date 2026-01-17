namespace AWSomeShop.API.Models.Entities;

public class AuditLog
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string AdminId { get; set; } = string.Empty;
    public string AdminName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string? EntityId { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public Administrator Admin { get; set; } = null!;
}
