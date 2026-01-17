using Microsoft.Extensions.Caching.Distributed;

namespace AWSomeShop.API.Middleware;

public class RateLimitMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IDistributedCache _cache;
    private readonly ILogger<RateLimitMiddleware> _logger;
    private const int MaxRequestsPerMinute = 60;

    public RateLimitMiddleware(
        RequestDelegate next,
        IDistributedCache cache,
        ILogger<RateLimitMiddleware> logger)
    {
        _next = next;
        _cache = cache;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var clientId = GetClientIdentifier(context);
        var cacheKey = $"ratelimit:{clientId}";

        var requestCountStr = await _cache.GetStringAsync(cacheKey);
        var requestCount = string.IsNullOrEmpty(requestCountStr) ? 0 : int.Parse(requestCountStr);

        if (requestCount >= MaxRequestsPerMinute)
        {
            _logger.LogWarning("Rate limit exceeded for client {ClientId}", clientId);
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await context.Response.WriteAsJsonAsync(new
            {
                message = "请求次数超过限制，请稍后再试",
                retryAfter = 60
            });
            return;
        }

        // 增加计数
        requestCount++;
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
        };
        await _cache.SetStringAsync(cacheKey, requestCount.ToString(), options);

        await _next(context);
    }

    private string GetClientIdentifier(HttpContext context)
    {
        // 优先使用用户ID，否则使用IP地址
        var userId = context.User?.Identity?.Name;
        if (!string.IsNullOrEmpty(userId))
        {
            return $"user:{userId}";
        }

        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return $"ip:{ipAddress}";
    }
}

public static class RateLimitMiddlewareExtensions
{
    public static IApplicationBuilder UseRateLimit(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RateLimitMiddleware>();
    }
}
