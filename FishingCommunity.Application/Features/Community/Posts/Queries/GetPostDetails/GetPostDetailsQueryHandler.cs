using FishingCommunity.Domain.Entities.Community;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FishingCommunity.Application.Features.Community.Posts.Queries.GetPostDetails;

public class GetPostDetailsQueryHandler : IRequestHandler<GetPostDetailsQuery, Result<PostDetailsDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPostDetailsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
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

        var dto = new PostDetailsDto
        {
            Id = post.Id,
            AuthorId = post.AuthorId,
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