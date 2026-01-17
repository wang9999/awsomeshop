namespace AWSomeShop.API.Models.DTOs;

public class NotificationDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? RelatedId { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}
