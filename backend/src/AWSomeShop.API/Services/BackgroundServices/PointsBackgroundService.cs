using AWSomeShop.API.Services.Interfaces;

namespace AWSomeShop.API.Services.BackgroundServices;

public class PointsBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PointsBackgroundService> _logger;
    private Timer? _monthlyTimer;
    private Timer? _dailyTimer;

    public PointsBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<PointsBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Points Background Service is starting");

        // Schedule monthly points grant (runs at 00:00 on the 1st of each month)
        ScheduleMonthlyTask(stoppingToken);

        // Schedule daily tasks (birthday points and expired points check at 00:00 daily)
        ScheduleDailyTask(stoppingToken);

        return Task.CompletedTask;
    }

    private void ScheduleMonthlyTask(CancellationToken stoppingToken)
    {
        var now = DateTime.UtcNow;
        var nextRun = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(1);
        
        // If today is the 1st and it's before midnight, run today
        if (now.Day == 1 && now.Hour == 0 && now.Minute < 5)
        {
            nextRun = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        }

        var delay = nextRun - now;
        
        _logger.LogInformation("Monthly points task scheduled to run at {NextRun} (in {Delay})", 
            nextRun, delay);

        _monthlyTimer = new Timer(
            async _ => await ExecuteMonthlyTask(stoppingToken),
            null,
            delay,
            TimeSpan.FromDays(30)); // Approximate monthly interval
    }

    private void ScheduleDailyTask(CancellationToken stoppingToken)
    {
        var now = DateTime.UtcNow;
        var nextRun = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc).AddDays(1);
        
        // If it's before 00:05 today, run today
        if (now.Hour == 0 && now.Minute < 5)
        {
            nextRun = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc);
        }

        var delay = nextRun - now;
        
        _logger.LogInformation("Daily points task scheduled to run at {NextRun} (in {Delay})", 
            nextRun, delay);

        _dailyTimer = new Timer(
            async _ => await ExecuteDailyTask(stoppingToken),
            null,
            delay,
            TimeSpan.FromDays(1)); // Daily interval
    }

    private async Task ExecuteMonthlyTask(CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
            return;

        try
        {
            _logger.LogInformation("Executing monthly points grant task");

            using var scope = _serviceProvider.CreateScope();
            var scheduledTaskService = scope.ServiceProvider.GetRequiredService<IScheduledTaskService>();
            
            await scheduledTaskService.GrantMonthlyPointsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing monthly points task");
        }
    }

    private async Task ExecuteDailyTask(CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
            return;

        try
        {
            _logger.LogInformation("Executing daily points tasks");

            using var scope = _serviceProvider.CreateScope();
            var scheduledTaskService = scope.ServiceProvider.GetRequiredService<IScheduledTaskService>();
            
            // Grant birthday points
            await scheduledTaskService.GrantBirthdayPointsAsync();
            
            // Process expired points
            await scheduledTaskService.ProcessExpiredPointsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing daily points tasks");
        }
    }

    public override void Dispose()
    {
        _monthlyTimer?.Dispose();
        _dailyTimer?.Dispose();
        base.Dispose();
    }
}
