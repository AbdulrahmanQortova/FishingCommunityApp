using FishingCommunity.Domain.Entities.Community;
using FishingCommunity.Domain.Exceptions;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Community.Posts.Commands.ToggleSavePost;

public class ToggleSavePostCommandHandler : IRequestHandler<ToggleSavePostCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ToggleSavePostCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(ToggleSavePostCommand request, CancellationToken cancellationToken)
    {
        var postExists = await _unitOfWork.Repository<Post>().AnyAsync(p => p.Id == request.PostId, cancellationToken);

        if (!postExists)
        {
            throw new NotFoundException(nameof(Post), request.PostId);
        }

        var existing = (await _unitOfWork.Repository<SavedPost>()
            .FindAsync(s => s.PostId == request.PostId && s.UserId == request.UserId, cancellationToken))
            .FirstOrDefault();

        if (existing is not null)
        {
            _unitOfWork.Repository<SavedPost>().Remove(existing);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(false, "Post removed from saved items.");
        }

        var savedPost = new SavedPost(request.PostId, request.UserId);
        await _unitOfWork.Repository<SavedPost>().AddAsync(savedPost, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "Post saved successfully.");
    }
}