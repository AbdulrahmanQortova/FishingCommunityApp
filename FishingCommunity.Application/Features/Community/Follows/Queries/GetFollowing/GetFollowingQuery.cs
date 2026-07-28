using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Community.Follows.Queries.GetFollowing;

public class GetFollowingQuery : IRequest<Result<List<Guid>>>
{
    public Guid UserId { get; set; }
}