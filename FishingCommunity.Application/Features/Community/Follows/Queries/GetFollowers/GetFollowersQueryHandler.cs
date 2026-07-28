using FishingCommunity.Domain.Entities.Community;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FishingCommunity.Application.Features.Community.Follows.Queries.GetFollowers;

public class GetFollowersQueryHandler : IRequestHandler<GetFollowersQuery, Result<List<Guid>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetFollowersQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<Guid>>> Handle(GetFollowersQuery request, CancellationToken cancellationToken)
    {
        var followerIds = await _unitOfWork.Repository<Follow>().Query()
            .Where(f => f.FollowedId == request.UserId)
            .Select(f => f.FollowerId)
            .ToListAsync(cancellationToken);

        return Result<List<Guid>>.Success(followerIds);
    }
}