using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Community.Posts.Queries.GetFeed;

public class GetFeedQuery : IRequest<Result<PaginatedList<PostSummaryDto>>>
{
    public Guid? RequestingUserId { get; set; } // Nullable — anonymous users can browse public feed too
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}