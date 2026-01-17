namespace AWSomeShop.API.Models.Entities;

public enum OrderStatus
{
    待确认,
    待发货,
    已发货,
    已完成,
    已取消
}

public class Order
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string OrderNumber { get; set; } = string.Empty;
    public string EmployeeId { get; set; } = string.Empty;
    public int TotalPoints { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.待确认;
    public string? AddressId { get; set; }
    public string? TrackingNumber { get; set; }
    public string? CancelReason { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Employee Employee { get; set; } = null!;
    public Address? Address { get; set; }
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
