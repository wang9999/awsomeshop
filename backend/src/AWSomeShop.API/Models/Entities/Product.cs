namespace AWSomeShop.API.Models.Entities;

public enum ProductCategory
{
    电子产品,
    生活用品,
    礼品卡,
    图书文具
}

public enum ProductStatus
{
    上架,
    下架
}

public class Product
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Points { get; set; }
    public int Stock { get; set; } = 0;
    public ProductCategory Category { get; set; }
    public ProductStatus Status { get; set; } = ProductStatus.下架;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    
    // 并发控制 - 乐观锁
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    // Navigation properties
    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    public ICollection<ProductTag> Tags { get; set; } = new List<ProductTag>();
}
