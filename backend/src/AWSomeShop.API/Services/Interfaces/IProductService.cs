using AWSomeShop.API.Models.DTOs;
using AWSomeShop.API.Models.Entities;

namespace AWSomeShop.API.Services.Interfaces;

public interface IProductService
{
    Task<ProductDto?> GetByIdAsync(string id);
    Task<ProductListResponse> GetAllAsync(ProductQueryParams queryParams);
    Task<ProductDto> CreateAsync(CreateProductRequest request, string createdBy);
    Task<ProductDto> UpdateAsync(string id, UpdateProductRequest request);
    Task DeleteAsync(string id);
    Task<ProductDto> UpdateStatusAsync(string id, ProductStatus status);
}
