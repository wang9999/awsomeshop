namespace AWSomeShop.API.Infrastructure.Cache;

public static class CacheKeys
{
    // Product cache keys
    public const string ProductList = "products:list";
    public const string ProductDetail = "products:detail:{0}";
    public const string ProductsByCategory = "products:category:{0}";
    
    // Employee cache keys
    public const string EmployeePoints = "employee:points:{0}";
    public const string EmployeeCart = "employee:cart:{0}";
    
    // Session cache keys
    public const string UserSession = "session:{0}";
    
    // Cache durations
    public static readonly TimeSpan ShortDuration = TimeSpan.FromMinutes(5);
    public static readonly TimeSpan MediumDuration = TimeSpan.FromMinutes(30);
    public static readonly TimeSpan LongDuration = TimeSpan.FromHours(2);
    public static readonly TimeSpan SessionDuration = TimeSpan.FromHours(2);
    
    public static string GetProductDetailKey(string productId) => string.Format(ProductDetail, productId);
    public static string GetProductsByCategoryKey(string category) => string.Format(ProductsByCategory, category);
    public static string GetEmployeePointsKey(string employeeId) => string.Format(EmployeePoints, employeeId);
    public static string GetEmployeeCartKey(string employeeId) => string.Format(EmployeeCart, employeeId);
    public static string GetUserSessionKey(string sessionId) => string.Format(UserSession, sessionId);
}
