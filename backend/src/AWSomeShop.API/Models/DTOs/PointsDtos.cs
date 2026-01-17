namespace AWSomeShop.API.Models.DTOs;

public class GrantPointsRequest
{
    public string EmployeeId { get; set; } = string.Empty;
    public int Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime? ExpiryDate { get; set; }
}

public class BatchGrantPointsRequest
{
    public List<string> EmployeeIds { get; set; } = new();
    public int Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime? ExpiryDate { get; set; }
}

public class DeductPointsRequest
{
    public string EmployeeId { get; set; } = string.Empty;
    public int Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class PointsBalanceResponse
{
    public int Balance { get; set; }
    public int ExpiringPoints { get; set; }
}

public class PointsTransactionResponse
{
    public string Id { get; set; } = string.Empty;
    public string EmployeeId { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public int Amount { get; set; }
    public string Type { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public string? OperatorId { get; set; }
    public string OperatorType { get; set; } = string.Empty;
    public int BalanceBefore { get; set; }
    public int BalanceAfter { get; set; }
    public string? RelatedOrderId { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class PointsTransactionListResponse
{
    public List<PointsTransactionResponse> Transactions { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}
