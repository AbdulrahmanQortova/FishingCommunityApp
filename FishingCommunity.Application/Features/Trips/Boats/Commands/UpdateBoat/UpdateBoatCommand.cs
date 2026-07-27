using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Trips.Boats.Commands.UpdateBoat;

public class UpdateBoatCommand : IRequest<Result>
{
    public Guid BoatId { get; set; }
    public Guid RequestingUserId { get; set; } // The user making the request (must be the owner)
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Capacity { get; set; }
}