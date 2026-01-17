using AWSomeShop.API.Infrastructure.DbContext;
using AWSomeShop.API.Models.DTOs;
using AWSomeShop.API.Models.Entities;
using AWSomeShop.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AWSomeShop.API.Services;

public class ShoppingCartService : IShoppingCartService
{
    private readonly AWSomeShopDbContext _context;
    private readonly ILogger<ShoppingCartService> _logger;

    public ShoppingCartService(AWSomeShopDbContext context, ILogger<ShoppingCartService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ShoppingCartDto> GetCartAsync(string employeeId)
    {
        var cartItems = await _context.ShoppingCarts
            .Include(c => c.Product)
            .ThenInclude(p => p.Images)
            .Where(c => c.EmployeeId == employeeId)
            .ToListAsync();

        var items = cartItems.Select(c => new CartItemDto
        {
            Id = c.Id,
            ProductId = c.ProductId,
            ProductName = c.Product.Name,
            ProductImage = c.Product.Images.FirstOrDefault()?.ImageUrl,
            Points = c.Product.Points,
            Quantity = c.Quantity,
            Stock = c.Product.Stock
        }).ToList();

        var totalPoints = items.Sum(i => i.Points * i.Quantity);

        return new ShoppingCartDto
        {
            Items = items,
            TotalPoints = totalPoints
        };
    }

    public async Task<ShoppingCartDto> AddItemAsync(string employeeId, string productId, int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException("Quantity must be greater than 0", nameof(quantity));
        }

        // Check if product exists and is available
        var product = await _context.Products.FindAsync(productId);
        if (product == null)
        {
            throw new InvalidOperationException("Product not found");
        }

        if (product.Status != ProductStatus.上架)
        {
            throw new InvalidOperationException("Product is not available");
        }

        if (product.Stock == 0)
        {
            throw new InvalidOperationException("Product is out of stock");
        }

        if (product.Stock < quantity)
        {
            throw new InvalidOperationException($"Insufficient stock. Available: {product.Stock}");
        }

        // Check if item already exists in cart
        var existingItem = await _context.ShoppingCarts
            .FirstOrDefaultAsync(c => c.EmployeeId == employeeId && c.ProductId == productId);

        if (existingItem != null)
        {
            // Update quantity
            var newQuantity = existingItem.Quantity + quantity;
            if (newQuantity > product.Stock)
            {
                throw new InvalidOperationException($"Insufficient stock. Available: {product.Stock}");
            }

            existingItem.Quantity = newQuantity;
            existingItem.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            // Add new item
            var cartItem = new ShoppingCart
            {
                EmployeeId = employeeId,
                ProductId = productId,
                Quantity = quantity,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.ShoppingCarts.Add(cartItem);
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("Added {Quantity} of product {ProductId} to cart for employee {EmployeeId}", 
            quantity, productId, employeeId);

        return await GetCartAsync(employeeId);
    }

    public async Task<ShoppingCartDto> UpdateItemQuantityAsync(string employeeId, string cartItemId, int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException("Quantity must be greater than 0", nameof(quantity));
        }

        var cartItem = await _context.ShoppingCarts
            .Include(c => c.Product)
            .FirstOrDefaultAsync(c => c.Id == cartItemId && c.EmployeeId == employeeId);

        if (cartItem == null)
        {
            throw new InvalidOperationException("Cart item not found");
        }

        if (cartItem.Product.Stock < quantity)
        {
            throw new InvalidOperationException($"Insufficient stock. Available: {cartItem.Product.Stock}");
        }

        cartItem.Quantity = quantity;
        cartItem.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        _logger.LogInformation("Updated cart item {CartItemId} quantity to {Quantity} for employee {EmployeeId}", 
            cartItemId, quantity, employeeId);

        return await GetCartAsync(employeeId);
    }

    public async Task<ShoppingCartDto> RemoveItemAsync(string employeeId, string cartItemId)
    {
        var cartItem = await _context.ShoppingCarts
            .FirstOrDefaultAsync(c => c.Id == cartItemId && c.EmployeeId == employeeId);

        if (cartItem == null)
        {
            throw new InvalidOperationException("Cart item not found");
        }

        _context.ShoppingCarts.Remove(cartItem);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Removed cart item {CartItemId} for employee {EmployeeId}", 
            cartItemId, employeeId);

        return await GetCartAsync(employeeId);
    }

    public async Task ClearCartAsync(string employeeId)
    {
        var cartItems = await _context.ShoppingCarts
            .Where(c => c.EmployeeId == employeeId)
            .ToListAsync();

        _context.ShoppingCarts.RemoveRange(cartItems);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Cleared cart for employee {EmployeeId}", employeeId);
    }

    public async Task<int> CalculateTotalPointsAsync(string employeeId)
    {
        var totalPoints = await _context.ShoppingCarts
            .Include(c => c.Product)
            .Where(c => c.EmployeeId == employeeId)
            .SumAsync(c => c.Product.Points * c.Quantity);

        return totalPoints;
    }
}
