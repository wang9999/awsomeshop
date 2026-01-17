using AWSomeShop.API.Infrastructure.DbContext;
using AWSomeShop.API.Models.Entities;
using AWSomeShop.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AWSomeShop.API.Repositories;

public class AddressRepository : IAddressRepository
{
    private readonly AWSomeShopDbContext _context;

    public AddressRepository(AWSomeShopDbContext context)
    {
        _context = context;
    }

    public async Task<List<Address>> GetAllByEmployeeIdAsync(string employeeId)
    {
        return await _context.Addresses
            .Where(a => a.EmployeeId == employeeId)
            .OrderByDescending(a => a.IsDefault)
            .ThenByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<Address?> GetByIdAsync(string id)
    {
        return await _context.Addresses.FindAsync(id);
    }

    public async Task<Address> CreateAsync(Address address)
    {
        _context.Addresses.Add(address);
        await _context.SaveChangesAsync();
        return address;
    }

    public async Task<Address> UpdateAsync(Address address)
    {
        address.UpdatedAt = DateTime.UtcNow;
        _context.Addresses.Update(address);
        await _context.SaveChangesAsync();
        return address;
    }

    public async Task DeleteAsync(string id)
    {
        var address = await _context.Addresses.FindAsync(id);
        if (address != null)
        {
            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Address?> GetDefaultByEmployeeIdAsync(string employeeId)
    {
        return await _context.Addresses
            .FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.IsDefault);
    }

    public async Task ClearDefaultForEmployeeAsync(string employeeId)
    {
        var addresses = await _context.Addresses
            .Where(a => a.EmployeeId == employeeId && a.IsDefault)
            .ToListAsync();

        foreach (var address in addresses)
        {
            address.IsDefault = false;
        }

        await _context.SaveChangesAsync();
    }
}
