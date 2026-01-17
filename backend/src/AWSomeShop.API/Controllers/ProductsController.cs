using AWSomeShop.API.Models.DTOs;
using AWSomeShop.API.Models.Entities;
using AWSomeShop.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AWSomeShop.API.Controllers;

[ApiController]
[Route("api")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductService productService, ILogger<ProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    /// <summary>
    /// Get product list (Employee - only shows online products)
    /// </summary>
    [HttpGet("products")]
    [Authorize(Roles = "Employee")]
    public async Task<ActionResult<ProductListResponse>> GetProducts([FromQuery] ProductQueryParams queryParams)
    {
        try
        {
            // Employees can only see online products
            queryParams.Status = "上架";
            
            var result = await _productService.GetAllAsync(queryParams);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting products");
            return StatusCode(500, new { message = "Failed to get products" });
        }
    }

    /// <summary>
    /// Get product details (Employee)
    /// </summary>
    [HttpGet("products/{id}")]
    [Authorize(Roles = "Employee")]
    public async Task<ActionResult<ProductDto>> GetProduct(string id)
    {
        try
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }

            // Employees can only see online products
            if (product.Status != "上架")
            {
                return NotFound(new { message = "Product not found" });
            }

            return Ok(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product {ProductId}", id);
            return StatusCode(500, new { message = "Failed to get product" });
        }
    }

    /// <summary>
    /// Admin: Get all products (including offline)
    /// </summary>
    [HttpGet("admin/products")]
    [Authorize(Roles = "SuperAdmin,ProductAdmin")]
    public async Task<ActionResult<ProductListResponse>> GetAllProductsAdmin([FromQuery] ProductQueryParams queryParams)
    {
        try
        {
            var result = await _productService.GetAllAsync(queryParams);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting products for admin");
            return StatusCode(500, new { message = "Failed to get products" });
        }
    }

    /// <summary>
    /// Admin: Get product details
    /// </summary>
    [HttpGet("admin/products/{id}")]
    [Authorize(Roles = "SuperAdmin,ProductAdmin")]
    public async Task<ActionResult<ProductDto>> GetProductAdmin(string id)
    {
        try
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }

            return Ok(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product {ProductId}", id);
            return StatusCode(500, new { message = "Failed to get product" });
        }
    }

    /// <summary>
    /// Admin: Create product
    /// </summary>
    [HttpPost("admin/products")]
    [Authorize(Roles = "SuperAdmin,ProductAdmin")]
    public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductRequest request)
    {
        try
        {
            var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(adminId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var product = await _productService.CreateAsync(request, adminId);
            return CreatedAtAction(nameof(GetProductAdmin), new { id = product.Id }, product);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            return StatusCode(500, new { message = "Failed to create product" });
        }
    }

    /// <summary>
    /// Admin: Update product
    /// </summary>
    [HttpPut("admin/products/{id}")]
    [Authorize(Roles = "SuperAdmin,ProductAdmin")]
    public async Task<ActionResult<ProductDto>> UpdateProduct(string id, [FromBody] UpdateProductRequest request)
    {
        try
        {
            var product = await _productService.UpdateAsync(id, request);
            return Ok(product);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product {ProductId}", id);
            return StatusCode(500, new { message = "Failed to update product" });
        }
    }

    /// <summary>
    /// Admin: Delete product
    /// </summary>
    [HttpDelete("admin/products/{id}")]
    [Authorize(Roles = "SuperAdmin,ProductAdmin")]
    public async Task<ActionResult> DeleteProduct(string id)
    {
        try
        {
            await _productService.DeleteAsync(id);
            return Ok(new { message = "Product deleted successfully" });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product {ProductId}", id);
            return StatusCode(500, new { message = "Failed to delete product" });
        }
    }

    /// <summary>
    /// Admin: Update product status (online/offline)
    /// </summary>
    [HttpPut("admin/products/{id}/status")]
    [Authorize(Roles = "SuperAdmin,ProductAdmin")]
    public async Task<ActionResult<ProductDto>> UpdateProductStatus(string id, [FromBody] UpdateProductStatusRequest request)
    {
        try
        {
            if (!Enum.TryParse<ProductStatus>(request.Status, out var status))
            {
                return BadRequest(new { message = $"Invalid status: {request.Status}" });
            }

            var product = await _productService.UpdateStatusAsync(id, status);
            return Ok(product);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product status {ProductId}", id);
            return StatusCode(500, new { message = "Failed to update product status" });
        }
    }
}

public class UpdateProductStatusRequest
{
    public string Status { get; set; } = string.Empty;
}
