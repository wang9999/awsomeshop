using AWSomeShop.API.Infrastructure.DbContext;
using AWSomeShop.API.Models.Entities;
using AWSomeShop.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AWSomeShop.API.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AWSomeShopDbContext _context;

    public OrderRepository(AWSomeShopDbContext context)
    {
        _context = context;
    }

    public async Task<Order?> GetByIdAsync(string id, bool includeItems = false, bool includeAddress = false)
    {
        var query = _context.Orders.AsQueryable();

        if (includeItems)
        {
            query = query.Include(o => o.Items);
        }

        if (includeAddress)
        {
            query = query.Include(o => o.Address);
        }

        return await query.FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<(List<Order> Orders, int TotalCount)> GetByEmployeeIdAsync(
        string employeeId,
        int pageNumber = 1,
        int pageSize = 10,
        OrderStatus? status = null,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        var query = _context.Orders
            .Include(o => o.Items)
            .Include(o => o.Address)
            .Where(o => o.EmployeeId == employeeId);

        if (status.HasValue)
        {
            query = query.Where(o => o.Status == status.Value);
        }

        if (startDate.HasValue)
        {
            query = query.Where(o => o.CreatedAt >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(o => o.CreatedAt <= endDate.Value);
        }

        var totalCount = await query.CountAsync();

        var orders = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (orders, totalCount);
    }

    public async Task<(List<Order> Orders, int TotalCount)> GetAllAsync(
        int pageNumber = 1,
        int pageSize = 10,
        OrderStatus? status = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? employeeId = null)
    {
        var query = _context.Orders
            .Include(o => o.Items)
            .Include(o => o.Address)
            .Include(o => o.Employee)
            .AsQueryable();

        if (!string.IsNullOrEmpty(employeeId))
        {
            query = query.Where(o => o.EmployeeId == employeeId);
        }

        if (status.HasValue)
        {
            query = query.Where(o => o.Status == status.Value);
        }

        if (startDate.HasValue)
        {
            query = query.Where(o => o.CreatedAt >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(o => o.CreatedAt <= endDate.Value);
        }

        var totalCount = await query.CountAsync();

        var orders = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (orders, totalCount);
    }

    public async Task<Order> CreateAsync(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<Order> UpdateAsync(Order order)
    {
        order.UpdatedAt = DateTime.UtcNow;
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<int> GetMonthlyOrderCountAsync(string employeeId, int year, int month)
    {
        var startDate = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        var endDate = startDate.AddMonths(1);

        return await _context.Orders
            .Where(o => o.EmployeeId == employeeId 
                && o.CreatedAt >= startDate 
                && o.CreatedAt < endDate
                && o.Status != OrderStatus.已取消)
            .CountAsync();
    }

    public async Task<bool> HasOrderedProductAsync(string employeeId, string productId)
    {
        return await _context.Orders
            .Where(o => o.EmployeeId == employeeId && o.Status != OrderStatus.已取消)
            .SelectMany(o => o.Items)
            .AnyAsync(item => item.ProductId == productId);
    }

    public async Task<Order?> GetByOrderNumberAsync(string orderNumber)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .Include(o => o.Address)
            .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
    }
}
