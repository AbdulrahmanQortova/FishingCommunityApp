using FishingCommunity.Domain.Entities.Community;
using FishingCommunity.Domain.Exceptions;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Community.Posts.Commands.EditPost;

public class EditPostCommandHandler : IRequestHandler<EditPostCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public EditPostCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(EditPostCommand request, CancellationToken cancellationToken)
    {
        var post = await _unitOfWork.Repository<Post>().GetByIdAsync(request.PostId, cancellationToken);

        if (post is null)
        {
            throw new NotFoundException(nameof(Post), request.PostId);
        }

        if (post.AuthorId != request.RequestingUserId)
        {
            return Result.Failure("You are not authorized to edit this post.");
        }

        post.Edit(request.Content);

        _unitOfWork.Repository<Post>().Update(post);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Post updated successfully.");
    }
}