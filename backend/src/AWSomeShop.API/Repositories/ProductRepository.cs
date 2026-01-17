using AWSomeShop.API.Infrastructure.DbContext;
using AWSomeShop.API.Models.Entities;
using AWSomeShop.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AWSomeShop.API.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AWSomeShopDbContext _context;

    public ProductRepository(AWSomeShopDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(string id, bool includeImages = false, bool includeTags = false)
    {
        var query = _context.Products.AsQueryable();

        if (includeImages)
        {
            query = query.Include(p => p.Images);
        }

        if (includeTags)
        {
            query = query.Include(p => p.Tags);
        }

        return await query.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<(List<Product> Products, int TotalCount)> GetAllAsync(
        int pageNumber = 1,
        int pageSize = 20,
        string? search = null,
        ProductCategory? category = null,
        int? minPoints = null,
        int? maxPoints = null,
        bool? inStock = null,
        ProductStatus? status = null)
    {
        var query = _context.Products
            .Include(p => p.Images)
            .Include(p => p.Tags)
            .AsQueryable();

        // Search filter
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p => p.Name.Contains(search) || 
                                   (p.Description != null && p.Description.Contains(search)));
        }

        // Category filter
        if (category.HasValue)
        {
            query = query.Where(p => p.Category == category.Value);
        }

        // Points range filter
        if (minPoints.HasValue)
        {
            query = query.Where(p => p.Points >= minPoints.Value);
        }

        if (maxPoints.HasValue)
        {
            query = query.Where(p => p.Points <= maxPoints.Value);
        }

        // Stock filter
        if (inStock.HasValue)
        {
            if (inStock.Value)
            {
                query = query.Where(p => p.Stock > 0);
            }
            else
            {
                query = query.Where(p => p.Stock == 0);
            }
        }

        // Status filter
        if (status.HasValue)
        {
            query = query.Where(p => p.Status == status.Value);
        }

        var totalCount = await query.CountAsync();

        var products = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (products, totalCount);
    }

    public async Task<Product> CreateAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<Product> UpdateAsync(Product product)
    {
        product.UpdatedAt = DateTime.UtcNow;
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task DeleteAsync(string id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(string id)
    {
        return await _context.Products.AnyAsync(p => p.Id == id);
    }
}
