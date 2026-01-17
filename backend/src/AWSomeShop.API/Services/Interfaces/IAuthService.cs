using AWSomeShop.API.Models.DTOs;

namespace AWSomeShop.API.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> EmployeeLoginAsync(EmployeeLoginRequest request);
    Task<LoginResponse> AdminLoginAsync(AdminLoginRequest request);
    Task<EmployeeInfoResponse?> GetCurrentEmployeeAsync(string employeeId);
}
