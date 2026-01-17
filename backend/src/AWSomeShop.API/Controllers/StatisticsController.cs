using AWSomeShop.API.Models.DTOs;
using AWSomeShop.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AWSomeShop.API.Controllers;

[ApiController]
[Route("api/admin/statistics")]
[Authorize(Roles = "SuperAdmin")]
public class StatisticsController : ControllerBase
{
    private readonly IStatisticsService _statisticsService;
    private readonly ILogger<StatisticsController> _logger;

    public StatisticsController(
        IStatisticsService statisticsService,
        ILogger<StatisticsController> logger)
    {
        _statisticsService = statisticsService;
        _logger = logger;
    }

    /// <summary>
    /// Get overview statistics
    /// </summary>
    [HttpGet("overview")]
    public async Task<ActionResult<OverviewStatisticsDto>> GetOverview([FromQuery] StatisticsQueryDto query)
    {
        try
        {
            var stats = await _statisticsService.GetOverviewStatisticsAsync(query.StartDate, query.EndDate);
            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting overview statistics");
            return StatusCode(500, new { message = "Failed to get statistics" });
        }
    }

    /// <summary>
    /// Get top products statistics
    /// </summary>
    [HttpGet("products")]
    public async Task<ActionResult<List<ProductStatisticsDto>>> GetTopProducts([FromQuery] StatisticsQueryDto query)
    {
        try
        {
            var stats = await _statisticsService.GetTopProductsAsync(query.TopN, query.StartDate, query.EndDate);
            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product statistics");
            return StatusCode(500, new { message = "Failed to get product statistics" });
        }
    }

    /// <summary>
    /// Get points trend statistics
    /// </summary>
    [HttpGet("points")]
    public async Task<ActionResult<List<PointsStatisticsDto>>> GetPointsTrend([FromQuery] StatisticsQueryDto query)
    {
        try
        {
            var stats = await _statisticsService.GetPointsTrendAsync(query.StartDate, query.EndDate);
            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting points statistics");
            return StatusCode(500, new { message = "Failed to get points statistics" });
        }
    }

    /// <summary>
    /// Get active employees statistics
    /// </summary>
    [HttpGet("employees")]
    public async Task<ActionResult<List<EmployeeStatisticsDto>>> GetActiveEmployees([FromQuery] StatisticsQueryDto query)
    {
        try
        {
            var stats = await _statisticsService.GetActiveEmployeesAsync(query.TopN, query.StartDate, query.EndDate);
            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting employee statistics");
            return StatusCode(500, new { message = "Failed to get employee statistics" });
        }
    }
}
