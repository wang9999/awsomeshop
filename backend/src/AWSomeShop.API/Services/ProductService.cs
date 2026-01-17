using AWSomeShop.API.Models.DTOs;
using AWSomeShop.API.Models.Entities;
using AWSomeShop.API.Repositories.Interfaces;
using AWSomeShop.API.Services.Interfaces;

namespace AWSomeShop.API.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IProductCacheService _cacheService;
    private readonly ILogger<ProductService> _logger;

    public ProductService(
        IProductRepository productRepository,
        IProductCacheService cacheService,
        ILogger<ProductService> logger)
    {
        _productRepository = productRepository;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<ProductDto?> GetByIdAsync(string id)
    {
        // 尝试从缓存获取
        var cachedProduct = await _cacheService.GetCachedProductAsync(id);
        if (cachedProduct != null)
        {
            return cachedProduct;
        }

        // 缓存未命中，从数据库获取
        var product = await _productRepository.GetByIdAsync(id, includeImages: true, includeTags: true);
        if (product == null)
        {
            return null;
        }

        var productDto = MapToDto(product);
        
        // 设置缓存
        await _cacheService.SetProductCacheAsync(id, productDto);
        
        return productDto;
    }

    public async Task<ProductListResponse> GetAllAsync(ProductQueryParams queryParams)
    {
        // 生成缓存键
        var cacheKey = _cacheService.GenerateProductListCacheKey(queryParams);
        
        // 尝试从缓存获取
        var cachedResponse = await _cacheService.GetCachedProductListAsync(cacheKey);
        if (cachedResponse != null)
        {
            return cachedResponse;
        }

        // 缓存未命中，从数据库查询
        ProductCategory? category = null;
        if (!string.IsNullOrEmpty(queryParams.Category) && 
            Enum.TryParse<ProductCategory>(queryParams.Category, out var parsedCategory))
        {
            category = parsedCategory;
        }

        ProductStatus? status = null;
        if (!string.IsNullOrEmpty(queryParams.Status) && 
            Enum.TryParse<ProductStatus>(queryParams.Status, out var parsedStatus))
        {
            status = parsedStatus;
        }

        var (products, totalCount) = await _productRepository.GetAllAsync(
            queryParams.PageNumber,
            queryParams.PageSize,
            queryParams.Search,
            category,
            queryParams.MinPoints,
            queryParams.MaxPoints,
            queryParams.InStock,
            status);

        var response = new ProductListResponse
        {
            Products = products.Select(MapToDto).ToList(),
            TotalCount = totalCount,
            PageNumber = queryParams.PageNumber,
            PageSize = queryParams.PageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)queryParams.PageSize)
        };

        // 设置缓存
        await _cacheService.SetProductListCacheAsync(cacheKey, response);

        return response;
    }

    public async Task<ProductDto> CreateAsync(CreateProductRequest request, string createdBy)
    {
        if (!Enum.TryParse<ProductCategory>(request.Category, out var category))
        {
            throw new ArgumentException($"Invalid category: {request.Category}");
        }

        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Points = request.Points,
            Stock = request.Stock,
            Category = category,
            Status = ProductStatus.下架, // Default to offline
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Add images
        if (request.ImageUrls != null && request.ImageUrls.Any())
        {
            foreach (var imageUrl in request.ImageUrls)
            {
                product.Images.Add(new ProductImage
                {
                    ProductId = product.Id,
                    ImageUrl = imageUrl
                });
            }
        }

        // Add tags
        if (request.Tags != null && request.Tags.Any())
        {
            foreach (var tagName in request.Tags)
            {
                product.Tags.Add(new ProductTag
                {
                    ProductId = product.Id,
                    TagName = tagName
                });
            }
        }

        var createdProduct = await _productRepository.CreateAsync(product);
        _logger.LogInformation("Product created: {ProductId} by {CreatedBy}", createdProduct.Id, createdBy);

        // 清除缓存
        await _cacheService.InvalidateProductCacheAsync();

        return MapToDto(createdProduct);
    }

    public async Task<ProductDto> UpdateAsync(string id, UpdateProductRequest request)
    {
        var product = await _productRepository.GetByIdAsync(id, includeImages: true, includeTags: true);
        if (product == null)
        {
            throw new InvalidOperationException($"Product with ID {id} not found");
        }

        if (!Enum.TryParse<ProductCategory>(request.Category, out var category))
        {
            throw new ArgumentException($"Invalid category: {request.Category}");
        }

        product.Name = request.Name;
        product.Description = request.Description;
        product.Points = request.Points;
        product.Stock = request.Stock;
        product.Category = category;

        // Update images
        product.Images.Clear();
        if (request.ImageUrls != null && request.ImageUrls.Any())
        {
            foreach (var imageUrl in request.ImageUrls)
            {
                product.Images.Add(new ProductImage
                {
                    ProductId = product.Id,
                    ImageUrl = imageUrl
                });
            }
        }

        // Update tags
        product.Tags.Clear();
        if (request.Tags != null && request.Tags.Any())
        {
            foreach (var tagName in request.Tags)
            {
                product.Tags.Add(new ProductTag
                {
                    ProductId = product.Id,
                    TagName = tagName
                });
            }
        }

        var updatedProduct = await _productRepository.UpdateAsync(product);
        _logger.LogInformation("Product updated: {ProductId}", updatedProduct.Id);

        // 清除缓存
        await _cacheService.InvalidateProductCacheAsync(id);

        return MapToDto(updatedProduct);
    }

    public async Task DeleteAsync(string id)
    {
        var exists = await _productRepository.ExistsAsync(id);
        if (!exists)
        {
            throw new InvalidOperationException($"Product with ID {id} not found");
        }

        await _productRepository.DeleteAsync(id);
        _logger.LogInformation("Product deleted: {ProductId}", id);
        
        // 清除缓存
        await _cacheService.InvalidateProductCacheAsync(id);
    }

    public async Task<ProductDto> UpdateStatusAsync(string id, ProductStatus status)
    {
        var product = await _productRepository.GetByIdAsync(id, includeImages: true, includeTags: true);
        if (product == null)
        {
            throw new InvalidOperationException($"Product with ID {id} not found");
        }

        product.Status = status;
        var updatedProduct = await _productRepository.UpdateAsync(product);
        
        _logger.LogInformation("Product status updated: {ProductId} to {Status}", id, status);

        // 清除缓存
        await _cacheService.InvalidateProductCacheAsync(id);

        return MapToDto(updatedProduct);
    }

    private static ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Points = product.Points,
            Stock = product.Stock,
            Category = product.Category.ToString(),
            Status = product.Status.ToString(),
            Images = product.Images.Select(i => i.ImageUrl).ToList(),
            Tags = product.Tags.Select(t => t.TagName).ToList(),
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }
}
