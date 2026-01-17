using Microsoft.Extensions.Caching.Distributed;
using System.Security.Cryptography;
using System.Text;

namespace AWSomeShop.API.Middleware;

public class DuplicateRequestMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IDistributedCache _cache;
    private readonly ILogger<DuplicateRequestMiddleware> _logger;

    public DuplicateRequestMiddleware(
        RequestDelegate next,
        IDistributedCache cache,
        ILogger<DuplicateRequestMiddleware> logger)
    {
        _next = next;
        _cache = cache;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 只对POST、PUT、DELETE请求进行防重复检查
        if (context.Request.Method == HttpMethods.Post ||
            context.Request.Method == HttpMethods.Put ||
            context.Request.Method == HttpMethods.Delete)
        {
            // 生成请求指纹
            var requestFingerprint = await GenerateRequestFingerprintAsync(context);
            var cacheKey = $"request:{requestFingerprint}";

            // 检查是否是重复请求
            var existingRequest = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(existingRequest))
            {
                _logger.LogWarning("Duplicate request detected: {Fingerprint}", requestFingerprint);
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsJsonAsync(new { message = "请求过于频繁，请稍后再试" });
                return;
            }

            // 标记请求已处理（5秒内不允许重复）
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5)
            };
            await _cache.SetStringAsync(cacheKey, DateTime.UtcNow.ToString(), options);
        }

        await _next(context);
    }

    private async Task<string> GenerateRequestFingerprintAsync(HttpContext context)
    {
        var request = context.Request;
        var userId = context.User?.Identity?.Name ?? "anonymous";
        var path = request.Path.Value ?? "";
        var method = request.Method;

        // 读取请求体
        string body = "";
        if (request.ContentLength > 0)
        {
            request.EnableBuffering();
            using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
            body = await reader.ReadToEndAsync();
            request.Body.Position = 0;
        }

        // 生成指纹
        var fingerprintSource = $"{userId}:{method}:{path}:{body}";
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(fingerprintSource));
        return Convert.ToBase64String(hashBytes);
    }
}

public static class DuplicateRequestMiddlewareExtensions
{
    public static IApplicationBuilder UseDuplicateRequestPrevention(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<DuplicateRequestMiddleware>();
    }
}
