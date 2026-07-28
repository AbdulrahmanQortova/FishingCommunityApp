using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Community.Follows.Commands.FollowUser;

public class FollowUserCommand : IRequest<Result>
{
    public Guid FollowerId { get; set; }
    public Guid FollowedId { get; set; }
}