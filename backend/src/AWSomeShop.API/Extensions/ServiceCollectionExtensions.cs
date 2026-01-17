using AWSomeShop.API.Infrastructure.Cache;
using AWSomeShop.API.Repositories;
using AWSomeShop.API.Repositories.Interfaces;
using AWSomeShop.API.Services;
using AWSomeShop.API.Services.Interfaces;
using StackExchange.Redis;

namespace AWSomeShop.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Core services
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<ICaptchaService, CaptchaService>();
        services.AddScoped<IProductCacheService, ProductCacheService>();
        
        // Business services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPointsService, PointsService>();
        services.AddScoped<IScheduledTaskService, ScheduledTaskService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IShoppingCartService, ShoppingCartService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IAddressService, AddressService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IAuditLogService, AuditLogService>();
        services.AddScoped<IStatisticsService, StatisticsService>();
        
        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        // Repositories
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IAddressRepository, AddressRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        // services.AddScoped<IPointsTransactionRepository, PointsTransactionRepository>();
        
        return services;
    }

    public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnectionString = configuration.GetConnectionString("Redis") ?? "localhost:6379";
        
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var configurationOptions = ConfigurationOptions.Parse(redisConnectionString);
            configurationOptions.AbortOnConnectFail = false;
            return ConnectionMultiplexer.Connect(configurationOptions);
        });
        
        services.AddSingleton<ICacheService, RedisCacheService>();
        
        return services;
    }
}
