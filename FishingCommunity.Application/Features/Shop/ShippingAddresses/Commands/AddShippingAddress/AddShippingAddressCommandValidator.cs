using FluentValidation;

namespace FishingCommunity.Application.Features.Shop.ShippingAddresses.Commands.AddShippingAddress;

public class AddShippingAddressCommandValidator : AbstractValidator<AddShippingAddressCommand>
{
    public AddShippingAddressCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();

        RuleFor(x => x.FullName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.PhoneNumber).NotEmpty().MaximumLength(20);
        RuleFor(x => x.AddressLine1).NotEmpty().MaximumLength(300);
        RuleFor(x => x.City).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Country).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PostalCode).MaximumLength(20).When(x => x.PostalCode is not null);
    }
}