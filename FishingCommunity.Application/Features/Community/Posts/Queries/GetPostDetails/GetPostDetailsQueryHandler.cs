using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Domain.Entities.Community;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FishingCommunity.Application.Features.Community.Posts.Queries.GetPostDetails;

public class GetPostDetailsQueryHandler : IRequestHandler<GetPostDetailsQuery, Result<PostDetailsDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityService _identityService;

    public GetPostDetailsQueryHandler(IUnitOfWork unitOfWork, IIdentityService identityService)
    {
        _unitOfWork = unitOfWork;
        _identityService = identityService;
    }

    public async Task<Result<PostDetailsDto>> Handle(GetPostDetailsQuery request, CancellationToken cancellationToken)
    {
        var post = await _unitOfWork.Repository<Post>().Query()
            .Where(p => p.Id == request.PostId)
            .Include(p => p.Comments)
            .Include(p => p.Reactions)
            .FirstOrDefaultAsync(cancellationToken);

        if (post is null)
        {
            return Result<PostDetailsDto>.Failure("Post not found.");
        }

        // Batch-resolve names for the post author + every commenter in one pass.
        var allUserIds = new List<Guid> { post.AuthorId };
        allUserIds.AddRange(post.Comments.Select(c => c.UserId));
        var uniqueUserIds = allUserIds.Distinct().ToList();

        var namesByUserId = new Dictionary<Guid, string>();

        foreach (var userId in uniqueUserIds)
        {
            var profile = await _identityService.GetUserProfileAsync(userId, cancellationToken);
            namesByUserId[userId] = profile is not null ? $"{profile.FirstName} {profile.LastName}".Trim() : "Angler";
        }

        var dto = new PostDetailsDto
        {
            Id = post.Id,
            AuthorId = post.AuthorId,
            AuthorName = namesByUserId.GetValueOrDefault(post.AuthorId, "Angler"),
            Content = post.Content,
            PhotoUrls = post.PhotoUrls.ToList(),
            IsEdited = post.IsEdited,
            CreatedDate = post.CreatedDate,
            LikesCount = post.LikesCount,
            DislikesCount = post.DislikesCount,
            Comments = post.Comments
                .OrderBy(c => c.CreatedDate)
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    UserId = c.UserId,
                    UserName = namesByUserId.GetValueOrDefault(c.UserId, "Angler"),
                    Content = c.Content,
                    ParentCommentId = c.ParentCommentId,
                    IsEdited = c.IsEdited,
                    IsRemoved = c.IsRemoved,
                    CreatedDate = c.CreatedDate
                }).ToList()
        };

        return Result<PostDetailsDto>.Success(dto);
    }
}