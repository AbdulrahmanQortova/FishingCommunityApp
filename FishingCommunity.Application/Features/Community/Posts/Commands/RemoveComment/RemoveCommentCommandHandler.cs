using FishingCommunity.Domain.Entities.Community;
using FishingCommunity.Domain.Exceptions;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FishingCommunity.Application.Features.Community.Posts.Commands.RemoveComment;

public class RemoveCommentCommandHandler : IRequestHandler<RemoveCommentCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public RemoveCommentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RemoveCommentCommand request, CancellationToken cancellationToken)
    {
        var post = await _unitOfWork.Repository<Post>().Query()
            .Where(p => p.Id == request.PostId)
            .Include(p => p.Comments)
            .FirstOrDefaultAsync(cancellationToken);

        if (post is null)
        {
            throw new NotFoundException(nameof(Post), request.PostId);
        }

        // Post.RemoveComment() throws BusinessRuleValidationException if the requesting
        // user is neither the comment author nor the post author — propagates to middleware.
        post.RemoveComment(request.CommentId, request.RequestingUserId);

        _unitOfWork.Repository<Post>().Update(post);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Comment removed successfully.");
    }
}