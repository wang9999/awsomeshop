using AWSomeShop.API.Models.DTOs;

namespace AWSomeShop.API.Services.Interfaces;

/// <summary>
/// 产品缓存服务接口
/// </summary>
public interface IProductCacheService
{
    /// <summary>
    /// 获取缓存的产品列表
    /// </summary>
    Task<ProductListResponse?> GetCachedProductListAsync(string cacheKey);

    /// <summary>
    /// 设置产品列表缓存
    /// </summary>
    Task SetProductListCacheAsync(string cacheKey, ProductListResponse response, TimeSpan? expiration = null);

    /// <summary>
    /// 获取缓存的产品详情
    /// </summary>
    Task<ProductDto?> GetCachedProductAsync(string productId);

    /// <summary>
    /// 设置产品详情缓存
    /// </summary>
    Task SetProductCacheAsync(string productId, ProductDto product, TimeSpan? expiration = null);

    /// <summary>
    /// 清除产品相关缓存
    /// </summary>
    Task InvalidateProductCacheAsync(string? productId = null);

    /// <summary>
    /// 生成产品列表缓存键
    /// </summary>
    string GenerateProductListCacheKey(ProductQueryParams queryParams);
}
