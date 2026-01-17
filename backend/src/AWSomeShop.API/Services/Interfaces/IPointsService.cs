namespace AWSomeShop.API.Services.Interfaces;

public interface IPointsService
{
    /// <summary>
    /// Get employee's current points balance
    /// </summary>
    Task<int> GetBalanceAsync(string employeeId);

    /// <summary>
    /// Get employee's points that will expire soon (within 30 days)
    /// </summary>
    Task<int> GetExpiringPointsAsync(string employeeId, int daysThreshold = 30);

    /// <summary>
    /// Add points to employee's account
    /// </summary>
    Task AddPointsAsync(string employeeId, int amount, string reason, string? operatorId = null, DateTime? expiryDate = null);

    /// <summary>
    /// Grant points to employee's account (alias for AddPointsAsync)
    /// </summary>
    Task GrantPointsAsync(string employeeId, int amount, string reason, string? operatorId = null, DateTime? expiryDate = null);

    /// <summary>
    /// Deduct points from employee's account
    /// </summary>
    Task DeductPointsAsync(string employeeId, int amount, string reason, string? operatorId = null, string? relatedOrderId = null);

    /// <summary>
    /// Check if employee has sufficient points
    /// </summary>
    Task<bool> HasSufficientPointsAsync(string employeeId, int requiredPoints);

    /// <summary>
    /// Get points transaction history with pagination
    /// </summary>
    Task<(List<Models.Entities.PointsTransaction> Transactions, int TotalCount)> GetTransactionHistoryAsync(
        string employeeId, 
        int pageNumber = 1, 
        int pageSize = 20,
        DateTime? startDate = null,
        DateTime? endDate = null);
}
