namespace AWSomeShop.API.Models.Entities;

public class Address
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string EmployeeId { get; set; } = string.Empty;
    public string ReceiverName { get; set; } = string.Empty;
    public string ReceiverPhone { get; set; } = string.Empty;
    public string? Province { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string DetailAddress { get; set; } = string.Empty;
    public bool IsDefault { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public Employee Employee { get; set; } = null!;
}
