using AWSomeShop.API.Models.DTOs;
using AWSomeShop.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AWSomeShop.API.Controllers;

[ApiController]
[Route("api/cart")]
[Authorize(Roles = "Employee")]
public class CartController : ControllerBase
{
    private readonly IShoppingCartService _cartService;
    private readonly ILogger<CartController> _logger;

    public CartController(IShoppingCartService cartService, ILogger<CartController> logger)
    {
        _cartService = cartService;
        _logger = logger;
    }

    /// <summary>
    /// Get current employee's shopping cart
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ShoppingCartDto>> GetCart()
    {
        try
        {
            var employeeId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(employeeId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var cart = await _cartService.GetCartAsync(employeeId);
            return Ok(cart);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cart");
            return StatusCode(500, new { message = "Failed to get cart" });
        }
    }

    /// <summary>
    /// Add item to cart
    /// </summary>
    [HttpPost("items")]
    public async Task<ActionResult<ShoppingCartDto>> AddItem([FromBody] AddCartItemDto request)
    {
        try
        {
            var employeeId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(employeeId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var cart = await _cartService.AddItemAsync(employeeId, request.ProductId, request.Quantity);
            return Ok(cart);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding item to cart");
            return StatusCode(500, new { message = "Failed to add item to cart" });
        }
    }

    /// <summary>
    /// Update cart item quantity
    /// </summary>
    [HttpPut("items/{id}")]
    public async Task<ActionResult<ShoppingCartDto>> UpdateItemQuantity(string id, [FromBody] UpdateCartItemDto request)
    {
        try
        {
            var employeeId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(employeeId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var cart = await _cartService.UpdateItemQuantityAsync(employeeId, id, request.Quantity);
            return Ok(cart);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating cart item");
            return StatusCode(500, new { message = "Failed to update cart item" });
        }
    }

    /// <summary>
    /// Remove item from cart
    /// </summary>
    [HttpDelete("items/{id}")]
    public async Task<ActionResult<ShoppingCartDto>> RemoveItem(string id)
    {
        try
        {
            var employeeId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(employeeId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var cart = await _cartService.RemoveItemAsync(employeeId, id);
            return Ok(cart);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cart item");
            return StatusCode(500, new { message = "Failed to remove cart item" });
        }
    }

    /// <summary>
    /// Clear all items from cart
    /// </summary>
    [HttpDelete]
    public async Task<ActionResult> ClearCart()
    {
        try
        {
            var employeeId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(employeeId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            await _cartService.ClearCartAsync(employeeId);
            return Ok(new { message = "Cart cleared successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing cart");
            return StatusCode(500, new { message = "Failed to clear cart" });
        }
    }
}

public class UpdateCartItemDto
{
    public int Quantity { get; set; }
}
