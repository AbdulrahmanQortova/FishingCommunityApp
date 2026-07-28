using FishingCommunity.Application.Common.Extensions;
using FishingCommunity.Domain.Entities.Community;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Community.Posts.Queries.GetFeed;

public class GetFeedQueryHandler : IRequestHandler<GetFeedQuery, Result<PaginatedList<PostSummaryDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetFeedQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PaginatedList<PostSummaryDto>>> Handle(GetFeedQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Post>().Query()
            .OrderByDescending(p => p.CreatedDate)
            .Select(p => new PostSummaryDto
            {
                Id = p.Id,
                AuthorId = p.AuthorId,
                Content = p.Content,
                // PhotoUrls comes from a backing field, but the reverse conversion is
                // set up in the EF configuration, so this projects correctly to SQL.
                PhotoUrls = p.PhotoUrls.ToList(),
                IsEdited = p.IsEdited,
                CreatedDate = p.CreatedDate,
                LikesCount = p.Reactions.Count(r => r.Type == Domain.Enums.ReactionType.Like),
                DislikesCount = p.Reactions.Count(r => r.Type == Domain.Enums.ReactionType.Dislike),
                CommentsCount = p.Comments.Count(c => !c.IsRemoved),
                CurrentUserReaction = request.RequestingUserId != null
                    ? p.Reactions.Where(r => r.UserId == request.RequestingUserId.Value).Select(r => r.Type.ToString()).FirstOrDefault()
                    : null,
                IsSavedByCurrentUser = request.RequestingUserId != null &&
                    _unitOfWork.Repository<SavedPost>().Query().Any(s => s.PostId == p.Id && s.UserId == request.RequestingUserId.Value)
            });

        var paginatedResult = await query.ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);

        return Result<PaginatedList<PostSummaryDto>>.Success(paginatedResult);
    }
}