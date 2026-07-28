using FishingCommunity.Domain.Entities.Community;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Community.Follows.Commands.UnfollowUser;

public class UnfollowUserCommandHandler : IRequestHandler<UnfollowUserCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public UnfollowUserCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UnfollowUserCommand request, CancellationToken cancellationToken)
    {
        var follow = (await _unitOfWork.Repository<Follow>()
            .FindAsync(f => f.FollowerId == request.FollowerId && f.FollowedId == request.FollowedId, cancellationToken))
            .FirstOrDefault();

        if (follow is null)
        {
            return Result.Success("You are not following this user."); // Idempotent, same as Logout pattern.
        }

        _unitOfWork.Repository<Follow>().Remove(follow);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Unfollowed successfully.");
    }
}