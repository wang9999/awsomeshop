namespace AWSomeShop.API.Models.Entities;

public class ProductImage
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ProductId { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public int DisplayOrder { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public Product Product { get; set; } = null!;
}
