namespace AWSomeShop.API.Services.Interfaces;

public interface IScheduledTaskService
{
    /// <summary>
    /// Grant monthly points to all active employees (500 points on 1st of each month)
    /// </summary>
    Task GrantMonthlyPointsAsync();

    /// <summary>
    /// Grant birthday points to employees (200 points on birthday)
    /// </summary>
    Task GrantBirthdayPointsAsync();

    /// <summary>
    /// Grant onboarding points to new employees (1000 points)
    /// </summary>
    Task GrantOnboardingPointsAsync(string employeeId);

    /// <summary>
    /// Process expired points (points older than 1 year)
    /// </summary>
    Task ProcessExpiredPointsAsync();
}
