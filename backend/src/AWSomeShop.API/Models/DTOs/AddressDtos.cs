namespace AWSomeShop.API.Models.DTOs;

public class AddressDto
{
    public string Id { get; set; } = string.Empty;
    public string ReceiverName { get; set; } = string.Empty;
    public string ReceiverPhone { get; set; } = string.Empty;
    public string? Province { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string DetailAddress { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
}

public class CreateAddressDto
{
    public string ReceiverName { get; set; } = string.Empty;
    public string ReceiverPhone { get; set; } = string.Empty;
    public string? Province { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string DetailAddress { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
}

public class UpdateAddressDto
{
    public string? ReceiverName { get; set; }
    public string? ReceiverPhone { get; set; }
    public string? Province { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? DetailAddress { get; set; }
    public bool? IsDefault { get; set; }
}
