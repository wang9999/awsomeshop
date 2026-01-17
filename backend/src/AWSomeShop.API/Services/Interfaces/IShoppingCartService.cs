using AWSomeShop.API.Models.DTOs;

namespace AWSomeShop.API.Services.Interfaces;

public interface IShoppingCartService
{
    /// <summary>
    /// Get employee's shopping cart
    /// </summary>
    Task<ShoppingCartDto> GetCartAsync(string employeeId);

    /// <summary>
    /// Add item to cart
    /// </summary>
    Task<ShoppingCartDto> AddItemAsync(string employeeId, string productId, int quantity);

    /// <summary>
    /// Update cart item quantity
    /// </summary>
    Task<ShoppingCartDto> UpdateItemQuantityAsync(string employeeId, string cartItemId, int quantity);

    /// <summary>
    /// Remove item from cart
    /// </summary>
    Task<ShoppingCartDto> RemoveItemAsync(string employeeId, string cartItemId);

    /// <summary>
    /// Clear all items from cart
    /// </summary>
    Task ClearCartAsync(string employeeId);

    /// <summary>
    /// Calculate total points for cart
    /// </summary>
    Task<int> CalculateTotalPointsAsync(string employeeId);
}
