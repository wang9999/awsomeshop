using AWSomeShop.API.Models.DTOs;
using AWSomeShop.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AWSomeShop.API.Controllers;

[ApiController]
[Route("api/addresses")]
[Authorize(Roles = "Employee")]
public class AddressesController : ControllerBase
{
    private readonly IAddressService _addressService;
    private readonly ILogger<AddressesController> _logger;

    public AddressesController(IAddressService addressService, ILogger<AddressesController> logger)
    {
        _addressService = addressService;
        _logger = logger;
    }

    /// <summary>
    /// Get all addresses for current employee
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<AddressDto>>> GetAddresses()
    {
        try
        {
            var employeeId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(employeeId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var addresses = await _addressService.GetAllByEmployeeIdAsync(employeeId);
            return Ok(addresses);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting addresses");
            return StatusCode(500, new { message = "Failed to get addresses" });
        }
    }

    /// <summary>
    /// Get address by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<AddressDto>> GetAddress(string id)
    {
        try
        {
            var employeeId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(employeeId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var address = await _addressService.GetByIdAsync(id, employeeId);
            if (address == null)
            {
                return NotFound(new { message = "Address not found" });
            }

            return Ok(address);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting address {AddressId}", id);
            return StatusCode(500, new { message = "Failed to get address" });
        }
    }

    /// <summary>
    /// Create new address
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<AddressDto>> CreateAddress([FromBody] CreateAddressDto dto)
    {
        try
        {
            var employeeId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(employeeId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var address = await _addressService.CreateAsync(dto, employeeId);
            return CreatedAtAction(nameof(GetAddress), new { id = address.Id }, address);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating address");
            return StatusCode(500, new { message = "Failed to create address" });
        }
    }

    /// <summary>
    /// Update address
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<AddressDto>> UpdateAddress(string id, [FromBody] UpdateAddressDto dto)
    {
        try
        {
            var employeeId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(employeeId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var address = await _addressService.UpdateAsync(id, dto, employeeId);
            return Ok(address);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating address {AddressId}", id);
            return StatusCode(500, new { message = "Failed to update address" });
        }
    }

    /// <summary>
    /// Delete address
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAddress(string id)
    {
        try
        {
            var employeeId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(employeeId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            await _addressService.DeleteAsync(id, employeeId);
            return Ok(new { message = "Address deleted successfully" });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting address {AddressId}", id);
            return StatusCode(500, new { message = "Failed to delete address" });
        }
    }

    /// <summary>
    /// Set address as default
    /// </summary>
    [HttpPut("{id}/default")]
    public async Task<ActionResult<AddressDto>> SetDefaultAddress(string id)
    {
        try
        {
            var employeeId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(employeeId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var address = await _addressService.SetDefaultAsync(id, employeeId);
            return Ok(address);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting default address {AddressId}", id);
            return StatusCode(500, new { message = "Failed to set default address" });
        }
    }
}
