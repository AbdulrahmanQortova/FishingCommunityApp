using FishingCommunity.Domain.Entities.Community;
using FishingCommunity.Domain.Exceptions;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FishingCommunity.Application.Features.Community.Posts.Commands.AddComment;

public class AddCommentCommandHandler : IRequestHandler<AddCommentCommand, Result<AddCommentResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddCommentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AddCommentResponse>> Handle(AddCommentCommand request, CancellationToken cancellationToken)
    {
        var post = await _unitOfWork.Repository<Post>().Query()
            .Where(p => p.Id == request.PostId)
            .Include(p => p.Comments)
            .FirstOrDefaultAsync(cancellationToken);

        if (post is null)
        {
            throw new NotFoundException(nameof(Post), request.PostId);
        }

        var comment = post.AddComment(request.UserId, request.Content, request.ParentCommentId);

        _unitOfWork.Repository<Post>().Update(post);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new AddCommentResponse
        {
            CommentId = comment.Id,
            PostId = post.Id,
            Content = comment.Content
        };

        return Result<AddCommentResponse>.Success(response, "Comment added successfully.");
    }
}