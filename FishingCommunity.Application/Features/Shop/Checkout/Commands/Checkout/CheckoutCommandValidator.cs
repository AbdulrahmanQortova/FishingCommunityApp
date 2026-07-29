using FluentValidation;

namespace FishingCommunity.Application.Features.Shop.Checkout.Commands.Checkout;

public class CheckoutCommandValidator : AbstractValidator<CheckoutCommand>
{
    public CheckoutCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.ShippingAddressId).NotEmpty();
    }
}