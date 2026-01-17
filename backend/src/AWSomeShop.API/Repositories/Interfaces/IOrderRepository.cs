using AWSomeShop.API.Models.Entities;

namespace AWSomeShop.API.Repositories.Interfaces;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(string id, bool includeItems = false, bool includeAddress = false);
    Task<(List<Order> Orders, int TotalCount)> GetByEmployeeIdAsync(
        string employeeId,
        int pageNumber = 1,
        int pageSize = 10,
        OrderStatus? status = null,
        DateTime? startDate = null,
        DateTime? endDate = null);
    Task<(List<Order> Orders, int TotalCount)> GetAllAsync(
        int pageNumber = 1,
        int pageSize = 10,
        OrderStatus? status = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? employeeId = null);
    Task<Order> CreateAsync(Order order);
    Task<Order> UpdateAsync(Order order);
    Task<int> GetMonthlyOrderCountAsync(string employeeId, int year, int month);
    Task<bool> HasOrderedProductAsync(string employeeId, string productId);
    Task<Order?> GetByOrderNumberAsync(string orderNumber);
}
