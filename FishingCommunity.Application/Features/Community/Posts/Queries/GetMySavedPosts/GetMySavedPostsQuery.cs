using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Community.Posts.Queries.GetMySavedPosts;

public class GetMySavedPostsQuery : IRequest<Result<List<SavedPostDto>>>
{
    public Guid UserId { get; set; }
}