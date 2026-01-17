using AWSomeShop.API.Infrastructure.DbContext;
using AWSomeShop.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AWSomeShop.API.Services;

public class ScheduledTaskService : IScheduledTaskService
{
    private readonly AWSomeShopDbContext _context;
    private readonly IPointsService _pointsService;
    private readonly ILogger<ScheduledTaskService> _logger;

    public ScheduledTaskService(
        AWSomeShopDbContext context,
        IPointsService pointsService,
        ILogger<ScheduledTaskService> logger)
    {
        _context = context;
        _pointsService = pointsService;
        _logger = logger;
    }

    public async Task GrantMonthlyPointsAsync()
    {
        try
        {
            _logger.LogInformation("Starting monthly points grant task");

            var activeEmployees = await _context.Employees
                .Where(e => e.IsActive)
                .ToListAsync();

            var successCount = 0;
            var failedCount = 0;
            var expiryDate = DateTime.UtcNow.AddYears(1); // Points expire in 1 year

            foreach (var employee in activeEmployees)
            {
                try
                {
                    await _pointsService.AddPointsAsync(
                        employee.Id,
                        500,
                        "每月自动发放",
                        operatorId: null,
                        expiryDate: expiryDate);
                    successCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to grant monthly points to employee {EmployeeId}", employee.Id);
                    failedCount++;
                }
            }

            _logger.LogInformation(
                "Monthly points grant completed. Success: {SuccessCount}, Failed: {FailedCount}",
                successCount, failedCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in monthly points grant task");
            throw;
        }
    }

    public async Task GrantBirthdayPointsAsync()
    {
        try
        {
            _logger.LogInformation("Starting birthday points grant task");

            var today = DateTime.UtcNow.Date;
            var birthdayEmployees = await _context.Employees
                .Where(e => e.IsActive 
                    && e.Birthday.HasValue
                    && e.Birthday.Value.Month == today.Month
                    && e.Birthday.Value.Day == today.Day)
                .ToListAsync();

            var successCount = 0;
            var failedCount = 0;
            var expiryDate = DateTime.UtcNow.AddYears(1); // Points expire in 1 year

            foreach (var employee in birthdayEmployees)
            {
                try
                {
                    // Check if birthday points already granted today
                    var alreadyGranted = await _context.PointsTransactions
                        .AnyAsync(t => t.EmployeeId == employee.Id
                            && t.Reason == "生日积分发放"
                            && t.CreatedAt.Date == today);

                    if (!alreadyGranted)
                    {
                        await _pointsService.AddPointsAsync(
                            employee.Id,
                            200,
                            "生日积分发放",
                            operatorId: null,
                            expiryDate: expiryDate);
                        successCount++;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to grant birthday points to employee {EmployeeId}", employee.Id);
                    failedCount++;
                }
            }

            _logger.LogInformation(
                "Birthday points grant completed. Success: {SuccessCount}, Failed: {FailedCount}",
                successCount, failedCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in birthday points grant task");
            throw;
        }
    }

    public async Task GrantOnboardingPointsAsync(string employeeId)
    {
        try
        {
            _logger.LogInformation("Granting onboarding points to employee {EmployeeId}", employeeId);

            var expiryDate = DateTime.UtcNow.AddYears(1); // Points expire in 1 year

            await _pointsService.AddPointsAsync(
                employeeId,
                1000,
                "入职欢迎积分",
                operatorId: null,
                expiryDate: expiryDate);

            _logger.LogInformation("Onboarding points granted to employee {EmployeeId}", employeeId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to grant onboarding points to employee {EmployeeId}", employeeId);
            throw;
        }
    }

    public async Task ProcessExpiredPointsAsync()
    {
        try
        {
            _logger.LogInformation("Starting expired points processing task");

            var now = DateTime.UtcNow;
            var expiredTransactions = await _context.PointsTransactions
                .Where(t => t.Type == Models.Entities.PointsTransactionType.发放
                    && t.ExpiryDate.HasValue
                    && t.ExpiryDate.Value <= now)
                .ToListAsync();

            var successCount = 0;
            var failedCount = 0;

            // Group by employee to process all expired points together
            var groupedByEmployee = expiredTransactions.GroupBy(t => t.EmployeeId);

            foreach (var group in groupedByEmployee)
            {
                try
                {
                    var employeeId = group.Key;
                    var totalExpiredPoints = group.Sum(t => t.Amount);

                    if (totalExpiredPoints > 0)
                    {
                        await _pointsService.DeductPointsAsync(
                            employeeId,
                            totalExpiredPoints,
                            $"积分过期扣除 ({group.Count()} 笔交易)",
                            operatorId: null);
                        successCount++;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process expired points for employee {EmployeeId}", group.Key);
                    failedCount++;
                }
            }

            _logger.LogInformation(
                "Expired points processing completed. Success: {SuccessCount}, Failed: {FailedCount}",
                successCount, failedCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in expired points processing task");
            throw;
        }
    }
}
