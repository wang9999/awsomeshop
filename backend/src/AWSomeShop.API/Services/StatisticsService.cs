using AWSomeShop.API.Infrastructure.DbContext;
using AWSomeShop.API.Models.DTOs;
using AWSomeShop.API.Models.Entities;
using AWSomeShop.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AWSomeShop.API.Services;

public class StatisticsService : IStatisticsService
{
    private readonly AWSomeShopDbContext _context;
    private readonly ILogger<StatisticsService> _logger;

    public StatisticsService(AWSomeShopDbContext context, ILogger<StatisticsService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<OverviewStatisticsDto> GetOverviewStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var now = DateTime.UtcNow;
        var start = startDate ?? now.AddMonths(-1);
        var end = endDate ?? now;

        // 总员工数
        var totalEmployees = await _context.Employees.CountAsync();

        // 总产品数
        var totalProducts = await _context.Products.CountAsync();

        // 在线产品数
        var onlineProducts = await _context.Products
            .Where(p => p.Status == ProductStatus.上架)
            .CountAsync();

        // 总订单数（时间范围内）
        var totalOrders = await _context.Orders
            .Where(o => o.CreatedAt >= start && o.CreatedAt <= end)
            .CountAsync();

        // 待处理订单数
        var pendingOrders = await _context.Orders
            .Where(o => o.Status == OrderStatus.待确认 || o.Status == OrderStatus.待发货)
            .CountAsync();

        // 总积分发放（时间范围内）
        var totalPointsIssued = await _context.PointsTransactions
            .Where(t => t.Type == PointsTransactionType.发放 
                && t.CreatedAt >= start 
                && t.CreatedAt <= end)
            .SumAsync(t => t.Amount);

        // 总积分消费（时间范围内）
        var totalPointsConsumed = await _context.PointsTransactions
            .Where(t => t.Type == PointsTransactionType.消费 
                && t.CreatedAt >= start 
                && t.CreatedAt <= end)
            .SumAsync(t => Math.Abs(t.Amount));

        // 活跃员工数（时间范围内有订单的员工）
        var activeEmployees = await _context.Orders
            .Where(o => o.CreatedAt >= start && o.CreatedAt <= end)
            .Select(o => o.EmployeeId)
            .Distinct()
            .CountAsync();

        return new OverviewStatisticsDto
        {
            TotalEmployees = totalEmployees,
            TotalProducts = totalProducts,
            OnlineProducts = onlineProducts,
            TotalOrders = totalOrders,
            PendingOrders = pendingOrders,
            TotalPointsIssued = totalPointsIssued,
            TotalPointsConsumed = totalPointsConsumed,
            ActiveEmployees = activeEmployees
        };
    }

    public async Task<List<ProductStatisticsDto>> GetTopProductsAsync(int topN = 10, DateTime? startDate = null, DateTime? endDate = null)
    {
        var now = DateTime.UtcNow;
        var start = startDate ?? now.AddMonths(-1);
        var end = endDate ?? now;

        var productStats = await _context.Orders
            .Where(o => o.CreatedAt >= start 
                && o.CreatedAt <= end 
                && o.Status != OrderStatus.已取消)
            .SelectMany(o => o.Items)
            .GroupBy(item => new { item.ProductId, item.ProductName })
            .Select(g => new ProductStatisticsDto
            {
                ProductId = g.Key.ProductId,
                ProductName = g.Key.ProductName,
                OrderCount = g.Count(),
                TotalQuantity = g.Sum(item => item.Quantity),
                TotalPoints = g.Sum(item => item.Points * item.Quantity)
            })
            .OrderByDescending(p => p.OrderCount)
            .Take(topN)
            .ToListAsync();

        return productStats;
    }

    public async Task<List<PointsStatisticsDto>> GetPointsTrendAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var now = DateTime.UtcNow;
        var start = startDate ?? now.AddMonths(-1);
        var end = endDate ?? now;

        var transactions = await _context.PointsTransactions
            .Where(t => t.CreatedAt >= start && t.CreatedAt <= end)
            .ToListAsync();

        var dailyStats = transactions
            .GroupBy(t => t.CreatedAt.Date)
            .Select(g => new PointsStatisticsDto
            {
                Date = g.Key,
                PointsIssued = g.Where(t => t.Type == PointsTransactionType.发放).Sum(t => t.Amount),
                PointsConsumed = Math.Abs(g.Where(t => t.Type == PointsTransactionType.消费).Sum(t => t.Amount)),
                NetChange = g.Sum(t => t.Amount)
            })
            .OrderBy(s => s.Date)
            .ToList();

        return dailyStats;
    }

    public async Task<List<EmployeeStatisticsDto>> GetActiveEmployeesAsync(int topN = 10, DateTime? startDate = null, DateTime? endDate = null)
    {
        var now = DateTime.UtcNow;
        var start = startDate ?? now.AddMonths(-1);
        var end = endDate ?? now;

        var employeeStats = await _context.Orders
            .Where(o => o.CreatedAt >= start 
                && o.CreatedAt <= end 
                && o.Status != OrderStatus.已取消)
            .GroupBy(o => o.Employee)
            .Select(g => new EmployeeStatisticsDto
            {
                EmployeeId = g.Key.Id,
                EmployeeName = g.Key.Name,
                Email = g.Key.Email,
                OrderCount = g.Count(),
                TotalPointsConsumed = g.Sum(o => o.TotalPoints),
                LastOrderDate = g.Max(o => o.CreatedAt)
            })
            .OrderByDescending(e => e.OrderCount)
            .Take(topN)
            .ToListAsync();

        return employeeStats;
    }
}
