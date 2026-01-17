using System.Security.Claims;

namespace AWSomeShop.API.Middleware;

/// <summary>
/// Middleware for role-based access control (RBAC)
/// </summary>
public class RoleAuthorizationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RoleAuthorizationMiddleware> _logger;

    public RoleAuthorizationMiddleware(RequestDelegate next, ILogger<RoleAuthorizationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip authorization for auth endpoints
        if (context.Request.Path.StartsWithSegments("/api/auth"))
        {
            await _next(context);
            return;
        }

        // Skip authorization for health check and swagger
        if (context.Request.Path.StartsWithSegments("/health") ||
            context.Request.Path.StartsWithSegments("/swagger"))
        {
            await _next(context);
            return;
        }

        // Check if user is authenticated
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var role = context.User.FindFirst(ClaimTypes.Role)?.Value;
            var path = context.Request.Path.Value?.ToLower() ?? "";

            // Admin endpoints require admin roles
            if (path.Contains("/api/admin"))
            {
                if (role != "SuperAdmin" && role != "ProductAdmin" && role != "PointsAdmin")
                {
                    _logger.LogWarning("Unauthorized access attempt to admin endpoint by role: {Role}", role);
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsJsonAsync(new { message = "权限不足" });
                    return;
                }

                // Product management - only SuperAdmin and ProductAdmin
                if (path.Contains("/api/admin/products") && role == "PointsAdmin")
                {
                    _logger.LogWarning("PointsAdmin attempted to access product management");
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsJsonAsync(new { message = "权限不足，无法访问产品管理功能" });
                    return;
                }

                // Points management - only SuperAdmin and PointsAdmin
                if (path.Contains("/api/admin/points") && role == "ProductAdmin")
                {
                    _logger.LogWarning("ProductAdmin attempted to access points management");
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsJsonAsync(new { message = "权限不足，无法访问积分管理功能" });
                    return;
                }
            }
        }

        await _next(context);
    }
}

/// <summary>
/// Extension method to add role authorization middleware
/// </summary>
public static class RoleAuthorizationMiddlewareExtensions
{
    public static IApplicationBuilder UseRoleAuthorization(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RoleAuthorizationMiddleware>();
    }
}
