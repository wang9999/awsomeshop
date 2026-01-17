namespace AWSomeShop.API.Services.Interfaces;

public interface ICaptchaService
{
    Task<string> GenerateCaptchaAsync(string sessionId);
    Task<bool> ValidateCaptchaAsync(string sessionId, string captcha);
}
