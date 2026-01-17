namespace AWSomeShop.API.Models.DTOs;

public class ProductDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Points { get; set; }
    public int Stock { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public List<string> Images { get; set; } = new();
    public List<string> Tags { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateProductRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Points { get; set; }
    public int Stock { get; set; }
    public string Category { get; set; } = string.Empty;
    public List<string>? ImageUrls { get; set; }
    public List<string>? Tags { get; set; }
}

public class UpdateProductRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Points { get; set; }
    public int Stock { get; set; }
    public string Category { get; set; } = string.Empty;
    public List<string>? ImageUrls { get; set; }
    public List<string>? Tags { get; set; }
}

public class ProductListResponse
{
    public List<ProductDto> Products { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}

public class ProductQueryParams
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Search { get; set; }
    public string? Category { get; set; }
    public int? MinPoints { get; set; }
    public int? MaxPoints { get; set; }
    public bool? InStock { get; set; }
    public string? Status { get; set; }
}
