using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Trips.Boats.Commands.DeleteBoat;

public class DeleteBoatCommand : IRequest<Result>
{
    public Guid BoatId { get; set; }
    public Guid RequestingUserId { get; set; }
}