using FluentValidation;

namespace FishingCommunity.Application.Features.Trips.Bookings.Commands.RequestBooking;

public class RequestBookingCommandValidator : AbstractValidator<RequestBookingCommand>
{
    public RequestBookingCommandValidator()
    {
        RuleFor(x => x.TripId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();

        RuleFor(x => x.SeatsRequested)
            .GreaterThan(0).WithMessage("You must request at least 1 seat.")
            .LessThanOrEqualTo(10).WithMessage("You cannot request more than 10 seats at once.");
    }
}