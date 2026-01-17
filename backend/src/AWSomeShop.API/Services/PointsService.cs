using AWSomeShop.API.Infrastructure.DbContext;
using AWSomeShop.API.Models.Entities;
using AWSomeShop.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AWSomeShop.API.Services;

public class PointsService : IPointsService
{
    private readonly AWSomeShopDbContext _context;
    private readonly ILogger<PointsService> _logger;

    public PointsService(AWSomeShopDbContext context, ILogger<PointsService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<int> GetBalanceAsync(string employeeId)
    {
        var employee = await _context.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == employeeId);

        if (employee == null)
        {
            throw new InvalidOperationException($"Employee with ID {employeeId} not found");
        }

        return employee.PointsBalance;
    }

    public async Task<int> GetExpiringPointsAsync(string employeeId, int daysThreshold = 30)
    {
        var thresholdDate = DateTime.UtcNow.AddDays(daysThreshold);

        var expiringPoints = await _context.PointsTransactions
            .AsNoTracking()
            .Where(t => t.EmployeeId == employeeId 
                && t.Type == PointsTransactionType.发放
                && t.ExpiryDate.HasValue
                && t.ExpiryDate.Value <= thresholdDate
                && t.ExpiryDate.Value > DateTime.UtcNow)
            .SumAsync(t => t.Amount);

        return expiringPoints;
    }

    public async Task AddPointsAsync(string employeeId, int amount, string reason, string? operatorId = null, DateTime? expiryDate = null)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Amount must be positive", nameof(amount));
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == employeeId);

            if (employee == null)
            {
                throw new InvalidOperationException($"Employee with ID {employeeId} not found");
            }

            var balanceBefore = employee.PointsBalance;
            employee.PointsBalance += amount;
            employee.UpdatedAt = DateTime.UtcNow;

            // Ensure balance is non-negative (invariant)
            if (employee.PointsBalance < 0)
            {
                throw new InvalidOperationException("Points balance cannot be negative");
            }

            var pointsTransaction = new PointsTransaction
            {
                EmployeeId = employeeId,
                Amount = amount,
                Type = PointsTransactionType.发放,
                Reason = reason,
                OperatorId = operatorId,
                OperatorType = operatorId != null ? OperatorType.Admin : OperatorType.System,
                BalanceBefore = balanceBefore,
                BalanceAfter = employee.PointsBalance,
                ExpiryDate = expiryDate,
                CreatedAt = DateTime.UtcNow
            };

            _context.PointsTransactions.Add(pointsTransaction);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation(
                "Added {Amount} points to employee {EmployeeId}. Balance: {BalanceBefore} -> {BalanceAfter}",
                amount, employeeId, balanceBefore, employee.PointsBalance);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task GrantPointsAsync(string employeeId, int amount, string reason, string? operatorId = null, DateTime? expiryDate = null)
    {
        // GrantPointsAsync is an alias for AddPointsAsync
        await AddPointsAsync(employeeId, amount, reason, operatorId, expiryDate);
    }

    public async Task DeductPointsAsync(string employeeId, int amount, string reason, string? operatorId = null, string? relatedOrderId = null)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Amount must be positive", nameof(amount));
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == employeeId);

            if (employee == null)
            {
                throw new InvalidOperationException($"Employee with ID {employeeId} not found");
            }

            var balanceBefore = employee.PointsBalance;

            // Check if employee has sufficient points
            if (employee.PointsBalance < amount)
            {
                throw new InvalidOperationException($"Insufficient points. Current balance: {employee.PointsBalance}, Required: {amount}");
            }

            employee.PointsBalance -= amount;
            employee.UpdatedAt = DateTime.UtcNow;

            // Ensure balance is non-negative (invariant)
            if (employee.PointsBalance < 0)
            {
                throw new InvalidOperationException("Points balance cannot be negative");
            }

            var transactionType = relatedOrderId != null 
                ? PointsTransactionType.消费 
                : PointsTransactionType.扣除;

            var pointsTransaction = new PointsTransaction
            {
                EmployeeId = employeeId,
                Amount = -amount, // Negative for deduction
                Type = transactionType,
                Reason = reason,
                OperatorId = operatorId,
                OperatorType = operatorId != null ? OperatorType.Admin : OperatorType.System,
                BalanceBefore = balanceBefore,
                BalanceAfter = employee.PointsBalance,
                RelatedOrderId = relatedOrderId,
                CreatedAt = DateTime.UtcNow
            };

            _context.PointsTransactions.Add(pointsTransaction);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation(
                "Deducted {Amount} points from employee {EmployeeId}. Balance: {BalanceBefore} -> {BalanceAfter}",
                amount, employeeId, balanceBefore, employee.PointsBalance);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> HasSufficientPointsAsync(string employeeId, int requiredPoints)
    {
        var balance = await GetBalanceAsync(employeeId);
        return balance >= requiredPoints;
    }

    public async Task<(List<PointsTransaction> Transactions, int TotalCount)> GetTransactionHistoryAsync(
        string employeeId, 
        int pageNumber = 1, 
        int pageSize = 20,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        var query = _context.PointsTransactions
            .AsNoTracking()
            .Where(t => t.EmployeeId == employeeId);

        if (startDate.HasValue)
        {
            query = query.Where(t => t.CreatedAt >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(t => t.CreatedAt <= endDate.Value);
        }

        var totalCount = await query.CountAsync();

        var transactions = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (transactions, totalCount);
    }
}
