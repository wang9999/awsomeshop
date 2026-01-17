namespace AWSomeShop.API.Models.Entities;

public class ProductTag
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ProductId { get; set; } = string.Empty;
    public string TagName { get; set; } = string.Empty;

    // Navigation property
    public Product Product { get; set; } = null!;
}
