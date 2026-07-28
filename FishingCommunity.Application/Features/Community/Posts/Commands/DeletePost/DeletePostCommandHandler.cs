using FishingCommunity.Domain.Entities.Community;
using FishingCommunity.Domain.Exceptions;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Community.Posts.Commands.DeletePost;

public class DeletePostCommandHandler : IRequestHandler<DeletePostCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeletePostCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeletePostCommand request, CancellationToken cancellationToken)
    {
        var post = await _unitOfWork.Repository<Post>().GetByIdAsync(request.PostId, cancellationToken);

        if (post is null)
        {
            throw new NotFoundException(nameof(Post), request.PostId);
        }

        if (post.AuthorId != request.RequestingUserId)
        {
            return Result.Failure("You are not authorized to delete this post.");
        }

        _unitOfWork.Repository<Post>().Remove(post); // Soft delete via interceptor.
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Post deleted successfully.");
    }
}