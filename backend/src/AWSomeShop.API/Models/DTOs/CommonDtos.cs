namespace AWSomeShop.API.Models.DTOs;

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public string? ErrorCode { get; set; }
}

public class ApiResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? ErrorCode { get; set; }
}
