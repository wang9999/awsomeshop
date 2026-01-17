namespace AWSomeShop.API.Models.DTOs;

public class OverviewStatisticsDto
{
    public int TotalEmployees { get; set; }
    public int TotalProducts { get; set; }
    public int TotalOrders { get; set; }
    public int TotalPointsIssued { get; set; }
    public int TotalPointsConsumed { get; set; }
    public int ActiveEmployees { get; set; }
    public int OnlineProducts { get; set; }
    public int PendingOrders { get; set; }
}

public class ProductStatisticsDto
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int OrderCount { get; set; }
    public int TotalQuantity { get; set; }
    public int TotalPoints { get; set; }
}

public class PointsStatisticsDto
{
    public DateTime Date { get; set; }
    public int PointsIssued { get; set; }
    public int PointsConsumed { get; set; }
    public int NetChange { get; set; }
}

public class EmployeeStatisticsDto
{
    public string EmployeeId { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int OrderCount { get; set; }
    public int TotalPointsConsumed { get; set; }
    public DateTime? LastOrderDate { get; set; }
}

public class StatisticsQueryDto
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int TopN { get; set; } = 10;
}
