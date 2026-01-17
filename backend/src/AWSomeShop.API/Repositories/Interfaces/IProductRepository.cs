using AWSomeShop.API.Models.Entities;

namespace AWSomeShop.API.Repositories.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(string id, bool includeImages = false, bool includeTags = false);
    Task<(List<Product> Products, int TotalCount)> GetAllAsync(
        int pageNumber = 1,
        int pageSize = 20,
        string? search = null,
        ProductCategory? category = null,
        int? minPoints = null,
        int? maxPoints = null,
        bool? inStock = null,
        ProductStatus? status = null);
    Task<Product> CreateAsync(Product product);
    Task<Product> UpdateAsync(Product product);
    Task DeleteAsync(string id);
    Task<bool> ExistsAsync(string id);
}
