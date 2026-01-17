using AWSomeShop.API.Models.Entities;

namespace AWSomeShop.API.Repositories.Interfaces;

public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(string id);
    Task<Employee?> GetByEmailAsync(string email);
    Task<Employee?> GetByEmployeeNumberAsync(string employeeNumber);
    Task<IEnumerable<Employee>> GetAllAsync();
    Task<Employee> CreateAsync(Employee employee);
    Task<Employee> UpdateAsync(Employee employee);
    Task<bool> DeleteAsync(string id);
    Task<bool> ExistsAsync(string id);
}
