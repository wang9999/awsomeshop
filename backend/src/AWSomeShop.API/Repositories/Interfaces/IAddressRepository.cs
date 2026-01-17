using AWSomeShop.API.Models.Entities;

namespace AWSomeShop.API.Repositories.Interfaces;

public interface IAddressRepository
{
    Task<List<Address>> GetAllByEmployeeIdAsync(string employeeId);
    Task<Address?> GetByIdAsync(string id);
    Task<Address> CreateAsync(Address address);
    Task<Address> UpdateAsync(Address address);
    Task DeleteAsync(string id);
    Task<Address?> GetDefaultByEmployeeIdAsync(string employeeId);
    Task ClearDefaultForEmployeeAsync(string employeeId);
}
