using FishingCommunity.Domain.Entities.Community;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FishingCommunity.Application.Features.Community.Follows.Queries.GetFollowing;

public class GetFollowingQueryHandler : IRequestHandler<GetFollowingQuery, Result<List<Guid>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetFollowingQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<Guid>>> Handle(GetFollowingQuery request, CancellationToken cancellationToken)
    {
        var followingIds = await _unitOfWork.Repository<Follow>().Query()
            .Where(f => f.FollowerId == request.UserId)
            .Select(f => f.FollowedId)
            .ToListAsync(cancellationToken);

        return Result<List<Guid>>.Success(followingIds);
    }
}