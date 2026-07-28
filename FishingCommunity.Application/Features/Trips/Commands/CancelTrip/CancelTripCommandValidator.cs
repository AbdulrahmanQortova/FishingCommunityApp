using FluentValidation;

namespace FishingCommunity.Application.Features.Trips.Commands.CancelTrip;

public class CancelTripCommandValidator : AbstractValidator<CancelTripCommand>
{
    public CancelTripCommandValidator()
    {
        RuleFor(x => x.TripId).NotEmpty();
        RuleFor(x => x.RequestingUserId).NotEmpty();

        RuleFor(x => x.Reason)
            .MaximumLength(500)
            .When(x => x.Reason is not null);
    }
}