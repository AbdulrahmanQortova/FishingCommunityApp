using FishingCommunity.Domain.Common;
using FishingCommunity.Domain.Exceptions;

namespace FishingCommunity.Domain.Entities.Shop;

public class ShippingAddress : BaseAuditableEntity
{
    public Guid UserId { get; private set; }

    public string FullName { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public string AddressLine1 { get; private set; } = string.Empty;
    public string? AddressLine2 { get; private set; }
    public string City { get; private set; } = string.Empty;
    public string? State { get; private set; }
    public string Country { get; private set; } = string.Empty;
    public string? PostalCode { get; private set; }
    public bool IsDefault { get; private set; }

    private ShippingAddress() { } // EF Core

    public ShippingAddress(
        Guid userId, string fullName, string phoneNumber, string addressLine1,
        string city, string country, string? addressLine2 = null, string? state = null, string? postalCode = null)
    {
        if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(addressLine1))
        {
            throw new BusinessRuleValidationException("Full name and address are required.");
        }

        UserId = userId;
        FullName = fullName;
        PhoneNumber = phoneNumber;
        AddressLine1 = addressLine1;
        AddressLine2 = addressLine2;
        City = city;
        State = state;
        Country = country;
        PostalCode = postalCode;
    }

    public void SetAsDefault() => IsDefault = true;
    public void UnsetDefault() => IsDefault = false;

    public void UpdateDetails(
        string fullName, string phoneNumber, string addressLine1, string city, string country,
        string? addressLine2, string? state, string? postalCode)
    {
        FullName = fullName;
        PhoneNumber = phoneNumber;
        AddressLine1 = addressLine1;
        AddressLine2 = addressLine2;
        City = city;
        State = state;
        Country = country;
        PostalCode = postalCode;
    }
}