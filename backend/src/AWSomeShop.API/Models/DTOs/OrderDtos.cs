namespace AWSomeShop.API.Models.DTOs;

public class OrderDto
{
    public string Id { get; set; } = string.Empty;
    public string OrderNumber { get; set; } = string.Empty;
    public List<OrderItemDto> Items { get; set; } = new();
    public int TotalPoints { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? TrackingNumber { get; set; }
    public AddressDto? Address { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class OrderItemDto
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string? ProductImage { get; set; }
    public int Points { get; set; }
    public int Quantity { get; set; }
}

public class OrderQueryDto
{
    public string? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class CreateOrderDto
{
    public List<CreateOrderItemDto> Items { get; set; } = new();
    public string AddressId { get; set; } = string.Empty;
    public string Captcha { get; set; } = string.Empty;
}

public class CreateOrderItemDto
{
    public string ProductId { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;
}
