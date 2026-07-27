using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Trips.Boats.Commands.CreateBoat;

public class CreateBoatCommand : IRequest<Result<CreateBoatResponse>>
{
    public Guid OwnerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string RegistrationNumber { get; set; } = string.Empty;
    public int Capacity { get; set; }
}