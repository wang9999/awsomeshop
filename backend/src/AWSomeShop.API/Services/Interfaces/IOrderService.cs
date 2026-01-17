using AWSomeShop.API.Models.DTOs;
using AWSomeShop.API.Models.Entities;

namespace AWSomeShop.API.Services.Interfaces;

public interface IOrderService
{
    Task<OrderDto> CreateOrderAsync(CreateOrderDto dto, string employeeId);
    Task<OrderDto?> GetOrderByIdAsync(string orderId, string employeeId);
    Task<(List<OrderDto> Orders, int TotalCount)> GetOrdersByEmployeeIdAsync(
        string employeeId,
        int pageNumber = 1,
        int pageSize = 10,
        OrderStatus? status = null,
        DateTime? startDate = null,
        DateTime? endDate = null);
    Task<OrderDto> CancelOrderAsync(string orderId, string employeeId, string? reason = null);
    Task<(List<OrderDto> Orders, int TotalCount)> GetAllOrdersAsync(
        int pageNumber = 1,
        int pageSize = 10,
        OrderStatus? status = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? employeeId = null);
    Task<OrderDto> UpdateOrderStatusAsync(string orderId, OrderStatus newStatus, string? trackingNumber = null);
}
