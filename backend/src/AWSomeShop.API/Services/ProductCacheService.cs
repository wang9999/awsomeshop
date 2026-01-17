using System.Text.Json;
using AWSomeShop.API.Infrastructure.Cache;
using AWSomeShop.API.Models.DTOs;
using AWSomeShop.API.Services.Interfaces;

namespace AWSomeShop.API.Services;

/// <summary>
/// 产品缓存服务实现
/// </summary>
public class ProductCacheService : IProductCacheService
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<ProductCacheService> _logger;
    private const string ProductListCachePrefix = "product:list:";
    private const string ProductDetailCachePrefix = "product:detail:";
    private const int DefaultCacheMinutes = 10; // 产品列表缓存10分钟
    private const int ProductDetailCacheMinutes = 30; // 产品详情缓存30分钟

    public ProductCacheService(ICacheService cacheService, ILogger<ProductCacheService> logger)
    {
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<ProductListResponse?> GetCachedProductListAsync(string cacheKey)
    {
        try
        {
            var cachedData = await _cacheService.GetAsync<string>(cacheKey);
            if (string.IsNullOrEmpty(cachedData))
            {
                return null;
            }

            var response = JsonSerializer.Deserialize<ProductListResponse>(cachedData);
            _logger.LogDebug("Product list cache hit: {CacheKey}", cacheKey);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cached product list: {CacheKey}", cacheKey);
            return null;
        }
    }

    public async Task SetProductListCacheAsync(string cacheKey, ProductListResponse response, TimeSpan? expiration = null)
    {
        try
        {
            var jsonData = JsonSerializer.Serialize(response);
            var cacheExpiration = expiration ?? TimeSpan.FromMinutes(DefaultCacheMinutes);
            await _cacheService.SetAsync(cacheKey, jsonData, cacheExpiration);
            _logger.LogDebug("Product list cached: {CacheKey}, expiration: {Expiration}", cacheKey, cacheExpiration);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting product list cache: {CacheKey}", cacheKey);
        }
    }

    public async Task<ProductDto?> GetCachedProductAsync(string productId)
    {
        try
        {
            var cacheKey = $"{ProductDetailCachePrefix}{productId}";
            var cachedData = await _cacheService.GetAsync<string>(cacheKey);
            if (string.IsNullOrEmpty(cachedData))
            {
                return null;
            }

            var product = JsonSerializer.Deserialize<ProductDto>(cachedData);
            _logger.LogDebug("Product detail cache hit: {ProductId}", productId);
            return product;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cached product: {ProductId}", productId);
            return null;
        }
    }

    public async Task SetProductCacheAsync(string productId, ProductDto product, TimeSpan? expiration = null)
    {
        try
        {
            var cacheKey = $"{ProductDetailCachePrefix}{productId}";
            var jsonData = JsonSerializer.Serialize(product);
            var cacheExpiration = expiration ?? TimeSpan.FromMinutes(ProductDetailCacheMinutes);
            await _cacheService.SetAsync(cacheKey, jsonData, cacheExpiration);
            _logger.LogDebug("Product detail cached: {ProductId}, expiration: {Expiration}", productId, cacheExpiration);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting product cache: {ProductId}", productId);
        }
    }

    public async Task InvalidateProductCacheAsync(string? productId = null)
    {
        try
        {
            if (!string.IsNullOrEmpty(productId))
            {
                // 清除特定产品的缓存
                var cacheKey = $"{ProductDetailCachePrefix}{productId}";
                await _cacheService.RemoveAsync(cacheKey);
                _logger.LogInformation("Product cache invalidated: {ProductId}", productId);
            }

            // 清除所有产品列表缓存（使用模式匹配）
            // 注意：这需要Redis的SCAN命令支持，简化实现中我们只记录日志
            _logger.LogInformation("Product list cache invalidation requested");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error invalidating product cache");
        }
    }

    public string GenerateProductListCacheKey(ProductQueryParams queryParams)
    {
        // 生成基于查询参数的缓存键
        var keyParts = new List<string>
        {
            ProductListCachePrefix,
            $"page:{queryParams.PageNumber}",
            $"size:{queryParams.PageSize}"
        };

        if (!string.IsNullOrEmpty(queryParams.Search))
        {
            keyParts.Add($"search:{queryParams.Search}");
        }

        if (!string.IsNullOrEmpty(queryParams.Category))
        {
            keyParts.Add($"cat:{queryParams.Category}");
        }

        if (queryParams.MinPoints.HasValue)
        {
            keyParts.Add($"min:{queryParams.MinPoints}");
        }

        if (queryParams.MaxPoints.HasValue)
        {
            keyParts.Add($"max:{queryParams.MaxPoints}");
        }

        if (queryParams.InStock.HasValue)
        {
            keyParts.Add($"stock:{queryParams.InStock}");
        }

        if (!string.IsNullOrEmpty(queryParams.Status))
        {
            keyParts.Add($"status:{queryParams.Status}");
        }

        return string.Join(":", keyParts);
    }
}
