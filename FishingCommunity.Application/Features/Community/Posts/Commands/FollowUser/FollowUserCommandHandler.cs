using FishingCommunity.Application.Common.Models;
using FishingCommunity.Domain.Entities.Community;
using FishingCommunity.Domain.Events.Community;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Community.Follows.Commands.FollowUser;

public class FollowUserCommandHandler : IRequestHandler<FollowUserCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublisher _publisher;

    public FollowUserCommandHandler(IUnitOfWork unitOfWork, IPublisher publisher)
    {
        _unitOfWork = unitOfWork;
        _publisher = publisher;
    }

    public async Task<Result> Handle(FollowUserCommand request, CancellationToken cancellationToken)
    {
        if (request.FollowerId == request.FollowedId)
        {
            return Result.Failure("You cannot follow yourself.");
        }

        var alreadyFollowing = await _unitOfWork.Repository<Follow>()
            .AnyAsync(f => f.FollowerId == request.FollowerId && f.FollowedId == request.FollowedId, cancellationToken);

        if (alreadyFollowing)
        {
            return Result.Success("You are already following this user.");
        }

        var follow = new Follow(request.FollowerId, request.FollowedId);

        await _unitOfWork.Repository<Follow>().AddAsync(follow, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _publisher.Publish(
            new DomainEventNotification<UserFollowedEvent>(new UserFollowedEvent(request.FollowerId, request.FollowedId)),
            cancellationToken);

        return Result.Success("You are now following this user.");
    }
}