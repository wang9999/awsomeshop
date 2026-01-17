namespace AWSomeShop.API.Models.DTOs;

public class ShoppingCartDto
{
    public List<CartItemDto> Items { get; set; } = new();
    public int TotalPoints { get; set; }
}

public class CartItemDto
{
    public string Id { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string? ProductImage { get; set; }
    public int Points { get; set; }
    public int Quantity { get; set; }
    public int Stock { get; set; }
}

public class AddCartItemDto
{
    public string ProductId { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;
}
