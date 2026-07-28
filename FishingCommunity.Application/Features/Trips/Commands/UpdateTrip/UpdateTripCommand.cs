using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Trips.Commands.UpdateTrip;

public class UpdateTripCommand : IRequest<Result>
{
    public Guid TripId { get; set; }
    public Guid RequestingUserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime DepartureDateTime { get; set; }
    public DateTime? EstimatedReturnDateTime { get; set; }
    public decimal PricePerPerson { get; set; }
}