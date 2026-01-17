using AWSomeShop.API.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;

namespace AWSomeShop.API.Services;

public class CaptchaService : ICaptchaService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<CaptchaService> _logger;
    private const int CaptchaLength = 4;
    private const int ExpirationMinutes = 5;

    public CaptchaService(IDistributedCache cache, ILogger<CaptchaService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<string> GenerateCaptchaAsync(string sessionId)
    {
        // 生成随机验证码
        var random = new Random();
        var captcha = new StringBuilder();
        for (int i = 0; i < CaptchaLength; i++)
        {
            captcha.Append(random.Next(0, 10));
        }

        var captchaCode = captcha.ToString();
        var cacheKey = $"captcha:{sessionId}";

        // 存储到缓存，5分钟过期
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(ExpirationMinutes)
        };

        await _cache.SetStringAsync(cacheKey, captchaCode, options);
        _logger.LogInformation("Captcha generated for session {SessionId}", sessionId);

        return captchaCode;
    }

    public async Task<bool> ValidateCaptchaAsync(string sessionId, string captcha)
    {
        var cacheKey = $"captcha:{sessionId}";
        var storedCaptcha = await _cache.GetStringAsync(cacheKey);

        if (string.IsNullOrEmpty(storedCaptcha))
        {
            _logger.LogWarning("Captcha not found or expired for session {SessionId}", sessionId);
            return false;
        }

        // 验证后删除验证码（一次性使用）
        await _cache.RemoveAsync(cacheKey);

        var isValid = string.Equals(storedCaptcha, captcha, StringComparison.OrdinalIgnoreCase);
        _logger.LogInformation("Captcha validation for session {SessionId}: {IsValid}", sessionId, isValid);

        return isValid;
    }
}
