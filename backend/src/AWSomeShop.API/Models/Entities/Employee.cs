namespace AWSomeShop.API.Models.Entities;

public class Employee
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string EmployeeNumber { get; set; } = string.Empty;
    public string? Department { get; set; }
    public DateTime? Birthday { get; set; }
    public int PointsBalance { get; set; } = 0;
    public int MonthlyRedeemCount { get; set; } = 0;
    public DateTime? LastRedeemResetDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<ShoppingCart> ShoppingCarts { get; set; } = new List<ShoppingCart>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Address> Addresses { get; set; } = new List<Address>();
    public ICollection<PointsTransaction> PointsTransactions { get; set; } = new List<PointsTransaction>();
}
