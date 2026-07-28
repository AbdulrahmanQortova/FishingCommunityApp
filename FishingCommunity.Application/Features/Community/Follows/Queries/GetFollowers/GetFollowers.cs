using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Community.Follows.Queries.GetFollowers;

public class GetFollowersQuery : IRequest<Result<List<Guid>>>
{
    public Guid UserId { get; set; }
}