using FishingCommunity.Domain.Entities.Community;
using FishingCommunity.Domain.Exceptions;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FishingCommunity.Application.Features.Community.Posts.Commands.ReactToPost;

public class ReactToPostCommandHandler : IRequestHandler<ReactToPostCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public ReactToPostCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ReactToPostCommand request, CancellationToken cancellationToken)
    {
        var post = await _unitOfWork.Repository<Post>().Query()
            .Where(p => p.Id == request.PostId)
            .Include(p => p.Reactions)
            .FirstOrDefaultAsync(cancellationToken);

        if (post is null)
        {
            throw new NotFoundException(nameof(Post), request.PostId);
        }

        post.React(request.UserId, request.Type);

        _unitOfWork.Repository<Post>().Update(post);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}