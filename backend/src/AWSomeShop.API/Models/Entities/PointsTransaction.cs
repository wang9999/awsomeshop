namespace AWSomeShop.API.Models.Entities;

public enum PointsTransactionType
{
    发放,
    扣除,
    消费,
    退回,
    过期
}

public enum OperatorType
{
    System,
    Admin
}

public class PointsTransaction
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string EmployeeId { get; set; } = string.Empty;
    public int Amount { get; set; }
    public PointsTransactionType Type { get; set; }
    public string? Reason { get; set; }
    public string? OperatorId { get; set; }
    public OperatorType OperatorType { get; set; } = OperatorType.System;
    public int BalanceBefore { get; set; }
    public int BalanceAfter { get; set; }
    public string? RelatedOrderId { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public Employee Employee { get; set; } = null!;
}
