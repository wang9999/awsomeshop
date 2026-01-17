using AWSomeShop.API.Models.DTOs;

namespace AWSomeShop.API.Services.Interfaces;

public interface IStatisticsService
{
    Task<OverviewStatisticsDto> GetOverviewStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<List<ProductStatisticsDto>> GetTopProductsAsync(int topN = 10, DateTime? startDate = null, DateTime? endDate = null);
    Task<List<PointsStatisticsDto>> GetPointsTrendAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<List<EmployeeStatisticsDto>> GetActiveEmployeesAsync(int topN = 10, DateTime? startDate = null, DateTime? endDate = null);
}
