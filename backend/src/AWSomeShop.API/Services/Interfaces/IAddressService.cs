using AWSomeShop.API.Models.DTOs;

namespace AWSomeShop.API.Services.Interfaces;

public interface IAddressService
{
    Task<List<AddressDto>> GetAllByEmployeeIdAsync(string employeeId);
    Task<AddressDto?> GetByIdAsync(string id, string employeeId);
    Task<AddressDto> CreateAsync(CreateAddressDto dto, string employeeId);
    Task<AddressDto> UpdateAsync(string id, UpdateAddressDto dto, string employeeId);
    Task DeleteAsync(string id, string employeeId);
    Task<AddressDto> SetDefaultAsync(string id, string employeeId);
}
