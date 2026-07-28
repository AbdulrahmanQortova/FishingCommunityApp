using FluentValidation;

namespace FishingCommunity.Application.Features.Trips.Commands.CreateTrip;

public class CreateTripCommandValidator : AbstractValidator<CreateTripCommand>
{
    public CreateTripCommandValidator()
    {
        RuleFor(x => x.OrganizerId).NotEmpty();
        RuleFor(x => x.BoatId).NotEmpty();

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Trip title is required.")
            .MaximumLength(150);

        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .When(x => x.Description is not null);

        RuleFor(x => x.LocationName)
            .NotEmpty().WithMessage("Location name is required.")
            .MaximumLength(200);

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90.");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180.");

        RuleFor(x => x.DepartureDateTime)
            .GreaterThan(DateTime.UtcNow).WithMessage("Departure date must be in the future.");

        RuleFor(x => x.EstimatedReturnDateTime)
            .GreaterThan(x => x.DepartureDateTime)
            .WithMessage("Estimated return must be after the departure time.")
            .When(x => x.EstimatedReturnDateTime is not null);

        RuleFor(x => x.Capacity)
            .GreaterThan(0).WithMessage("Capacity must be greater than zero.");

        RuleFor(x => x.PricePerPerson)
            .GreaterThanOrEqualTo(0).WithMessage("Price cannot be negative.");
    }
}