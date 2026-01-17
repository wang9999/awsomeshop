using AWSomeShop.API.Models.DTOs;
using AWSomeShop.API.Models.Entities;
using AWSomeShop.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AWSomeShop.API.Controllers;

[ApiController]
[Route("api")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    /// <summary>
    /// Employee: Create order
    /// </summary>
    [HttpPost("orders")]
    [Authorize(Roles = "Employee")]
    public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderDto dto)
    {
        try
        {
            var employeeId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(employeeId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var order = await _orderService.CreateOrderAsync(dto, employeeId);
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order");
            return StatusCode(500, new { message = "Failed to create order" });
        }
    }

    /// <summary>
    /// Employee: Get order by ID
    /// </summary>
    [HttpGet("orders/{id}")]
    [Authorize(Roles = "Employee")]
    public async Task<ActionResult<OrderDto>> GetOrder(string id)
    {
        try
        {
            var employeeId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(employeeId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var order = await _orderService.GetOrderByIdAsync(id, employeeId);
            if (order == null)
            {
                return NotFound(new { message = "Order not found" });
            }

            return Ok(order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting order {OrderId}", id);
            return StatusCode(500, new { message = "Failed to get order" });
        }
    }

    /// <summary>
    /// Employee: Get my orders
    /// </summary>
    [HttpGet("orders")]
    [Authorize(Roles = "Employee")]
    public async Task<ActionResult<object>> GetMyOrders([FromQuery] OrderQueryDto query)
    {
        try
        {
            var employeeId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(employeeId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            OrderStatus? status = null;
            if (!string.IsNullOrEmpty(query.Status) && Enum.TryParse<OrderStatus>(query.Status, out var parsedStatus))
            {
                status = parsedStatus;
            }

            var (orders, totalCount) = await _orderService.GetOrdersByEmployeeIdAsync(
                employeeId,
                query.Page,
                query.PageSize,
                status,
                query.StartDate,
                query.EndDate);

            return Ok(new
            {
                orders,
                totalCount,
                page = query.Page,
                pageSize = query.PageSize,
                totalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting orders");
            return StatusCode(500, new { message = "Failed to get orders" });
        }
    }

    /// <summary>
    /// Employee: Cancel order
    /// </summary>
    [HttpPut("orders/{id}/cancel")]
    [Authorize(Roles = "Employee")]
    public async Task<ActionResult<OrderDto>> CancelOrder(string id, [FromBody] CancelOrderRequest? request = null)
    {
        try
        {
            var employeeId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(employeeId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var order = await _orderService.CancelOrderAsync(id, employeeId, request?.Reason);
            return Ok(order);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling order {OrderId}", id);
            return StatusCode(500, new { message = "Failed to cancel order" });
        }
    }

    /// <summary>
    /// Admin: Get all orders
    /// </summary>
    [HttpGet("admin/orders")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<ActionResult<object>> GetAllOrders([FromQuery] OrderQueryDto query, [FromQuery] string? employeeId = null)
    {
        try
        {
            OrderStatus? status = null;
            if (!string.IsNullOrEmpty(query.Status) && Enum.TryParse<OrderStatus>(query.Status, out var parsedStatus))
            {
                status = parsedStatus;
            }

            var (orders, totalCount) = await _orderService.GetAllOrdersAsync(
                query.Page,
                query.PageSize,
                status,
                query.StartDate,
                query.EndDate,
                employeeId);

            return Ok(new
            {
                orders,
                totalCount,
                page = query.Page,
                pageSize = query.PageSize,
                totalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all orders");
            return StatusCode(500, new { message = "Failed to get orders" });
        }
    }

    /// <summary>
    /// Admin: Update order status
    /// </summary>
    [HttpPut("admin/orders/{id}/status")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<ActionResult<OrderDto>> UpdateOrderStatus(string id, [FromBody] UpdateOrderStatusRequest request)
    {
        try
        {
            if (!Enum.TryParse<OrderStatus>(request.Status, out var status))
            {
                return BadRequest(new { message = $"Invalid status: {request.Status}" });
            }

            var order = await _orderService.UpdateOrderStatusAsync(id, status, request.TrackingNumber);
            return Ok(order);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating order status {OrderId}", id);
            return StatusCode(500, new { message = "Failed to update order status" });
        }
    }
}

public class CancelOrderRequest
{
    public string? Reason { get; set; }
}

public class UpdateOrderStatusRequest
{
    public string Status { get; set; } = string.Empty;
    public string? TrackingNumber { get; set; }
}
