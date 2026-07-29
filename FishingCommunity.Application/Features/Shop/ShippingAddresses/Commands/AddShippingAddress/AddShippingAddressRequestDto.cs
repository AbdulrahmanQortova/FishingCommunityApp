// Features/Shop/ShippingAddresses/Commands/AddShippingAddress/AddShippingAddressRequestDto.cs
namespace FishingCommunity.Application.Features.Shop.ShippingAddresses.Commands.AddShippingAddress;

public class AddShippingAddressRequestDto
{
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string? State { get; set; }
    public string Country { get; set; } = string.Empty;
    public string? PostalCode { get; set; }
    public bool SetAsDefault { get; set; }
}