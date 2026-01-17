using AWSomeShop.API.Models.DTOs;
using AWSomeShop.API.Models.Entities;
using AWSomeShop.API.Repositories.Interfaces;
using AWSomeShop.API.Services.Interfaces;

namespace AWSomeShop.API.Services;

public class AddressService : IAddressService
{
    private readonly IAddressRepository _addressRepository;
    private readonly ILogger<AddressService> _logger;

    public AddressService(IAddressRepository addressRepository, ILogger<AddressService> logger)
    {
        _addressRepository = addressRepository;
        _logger = logger;
    }

    public async Task<List<AddressDto>> GetAllByEmployeeIdAsync(string employeeId)
    {
        var addresses = await _addressRepository.GetAllByEmployeeIdAsync(employeeId);
        return addresses.Select(MapToDto).ToList();
    }

    public async Task<AddressDto?> GetByIdAsync(string id, string employeeId)
    {
        var address = await _addressRepository.GetByIdAsync(id);
        if (address == null || address.EmployeeId != employeeId)
        {
            return null;
        }
        return MapToDto(address);
    }

    public async Task<AddressDto> CreateAsync(CreateAddressDto dto, string employeeId)
    {
        // If this is set as default, clear other defaults first
        if (dto.IsDefault)
        {
            await _addressRepository.ClearDefaultForEmployeeAsync(employeeId);
        }

        var address = new Address
        {
            EmployeeId = employeeId,
            ReceiverName = dto.ReceiverName,
            ReceiverPhone = dto.ReceiverPhone,
            Province = dto.Province,
            City = dto.City,
            District = dto.District,
            DetailAddress = dto.DetailAddress,
            IsDefault = dto.IsDefault,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await _addressRepository.CreateAsync(address);
        _logger.LogInformation("Address created: {AddressId} for employee {EmployeeId}", created.Id, employeeId);

        return MapToDto(created);
    }

    public async Task<AddressDto> UpdateAsync(string id, UpdateAddressDto dto, string employeeId)
    {
        var address = await _addressRepository.GetByIdAsync(id);
        if (address == null || address.EmployeeId != employeeId)
        {
            throw new InvalidOperationException($"Address with ID {id} not found");
        }

        // If setting as default, clear other defaults first
        if (dto.IsDefault == true)
        {
            await _addressRepository.ClearDefaultForEmployeeAsync(employeeId);
        }

        if (dto.ReceiverName != null) address.ReceiverName = dto.ReceiverName;
        if (dto.ReceiverPhone != null) address.ReceiverPhone = dto.ReceiverPhone;
        if (dto.Province != null) address.Province = dto.Province;
        if (dto.City != null) address.City = dto.City;
        if (dto.District != null) address.District = dto.District;
        if (dto.DetailAddress != null) address.DetailAddress = dto.DetailAddress;
        if (dto.IsDefault.HasValue) address.IsDefault = dto.IsDefault.Value;

        var updated = await _addressRepository.UpdateAsync(address);
        _logger.LogInformation("Address updated: {AddressId}", id);

        return MapToDto(updated);
    }

    public async Task DeleteAsync(string id, string employeeId)
    {
        var address = await _addressRepository.GetByIdAsync(id);
        if (address == null || address.EmployeeId != employeeId)
        {
            throw new InvalidOperationException($"Address with ID {id} not found");
        }

        await _addressRepository.DeleteAsync(id);
        _logger.LogInformation("Address deleted: {AddressId}", id);
    }

    public async Task<AddressDto> SetDefaultAsync(string id, string employeeId)
    {
        var address = await _addressRepository.GetByIdAsync(id);
        if (address == null || address.EmployeeId != employeeId)
        {
            throw new InvalidOperationException($"Address with ID {id} not found");
        }

        // Clear other defaults first
        await _addressRepository.ClearDefaultForEmployeeAsync(employeeId);

        address.IsDefault = true;
        var updated = await _addressRepository.UpdateAsync(address);
        _logger.LogInformation("Address set as default: {AddressId}", id);

        return MapToDto(updated);
    }

    private static AddressDto MapToDto(Address address)
    {
        return new AddressDto
        {
            Id = address.Id,
            ReceiverName = address.ReceiverName,
            ReceiverPhone = address.ReceiverPhone,
            Province = address.Province,
            City = address.City,
            District = address.District,
            DetailAddress = address.DetailAddress,
            IsDefault = address.IsDefault
        };
    }
}
