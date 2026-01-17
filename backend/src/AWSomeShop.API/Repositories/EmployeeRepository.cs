using AWSomeShop.API.Infrastructure.DbContext;
using AWSomeShop.API.Models.Entities;
using AWSomeShop.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AWSomeShop.API.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly AWSomeShopDbContext _context;

    public EmployeeRepository(AWSomeShopDbContext context)
    {
        _context = context;
    }

    public async Task<Employee?> GetByIdAsync(string id)
    {
        return await _context.Employees
            .Include(e => e.Addresses)
            .FirstOrDefaultAsync(e => e.Id == id && e.IsActive);
    }

    public async Task<Employee?> GetByEmailAsync(string email)
    {
        return await _context.Employees
            .FirstOrDefaultAsync(e => e.Email == email && e.IsActive);
    }

    public async Task<Employee?> GetByEmployeeNumberAsync(string employeeNumber)
    {
        return await _context.Employees
            .FirstOrDefaultAsync(e => e.EmployeeNumber == employeeNumber && e.IsActive);
    }

    public async Task<IEnumerable<Employee>> GetAllAsync()
    {
        return await _context.Employees
            .Where(e => e.IsActive)
            .ToListAsync();
    }

    public async Task<Employee> CreateAsync(Employee employee)
    {
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();
        return employee;
    }

    public async Task<Employee> UpdateAsync(Employee employee)
    {
        employee.UpdatedAt = DateTime.UtcNow;
        _context.Employees.Update(employee);
        await _context.SaveChangesAsync();
        return employee;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null) return false;

        employee.IsActive = false;
        employee.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(string id)
    {
        return await _context.Employees.AnyAsync(e => e.Id == id && e.IsActive);
    }
}
