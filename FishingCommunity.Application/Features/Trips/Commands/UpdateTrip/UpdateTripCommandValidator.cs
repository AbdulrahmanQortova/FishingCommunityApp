using FluentValidation;

namespace FishingCommunity.Application.Features.Trips.Commands.UpdateTrip;

public class UpdateTripCommandValidator : AbstractValidator<UpdateTripCommand>
{
    public UpdateTripCommandValidator()
    {
        RuleFor(x => x.TripId).NotEmpty();
        RuleFor(x => x.RequestingUserId).NotEmpty();

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Trip title is required.")
            .MaximumLength(150);

        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .When(x => x.Description is not null);

        RuleFor(x => x.DepartureDateTime)
            .GreaterThan(DateTime.UtcNow).WithMessage("Departure date must be in the future.");

        RuleFor(x => x.EstimatedReturnDateTime)
            .GreaterThan(x => x.DepartureDateTime)
            .When(x => x.EstimatedReturnDateTime is not null);

        RuleFor(x => x.PricePerPerson)
            .GreaterThanOrEqualTo(0);
    }
}