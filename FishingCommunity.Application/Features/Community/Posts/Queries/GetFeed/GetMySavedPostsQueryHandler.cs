using FishingCommunity.Domain.Entities.Community;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FishingCommunity.Application.Features.Community.Posts.Queries.GetMySavedPosts;

public class GetMySavedPostsQueryHandler : IRequestHandler<GetMySavedPostsQuery, Result<List<SavedPostDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetMySavedPostsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<SavedPostDto>>> Handle(GetMySavedPostsQuery request, CancellationToken cancellationToken)
    {
        var savedPosts = await _unitOfWork.Repository<SavedPost>().Query()
            .Where(s => s.UserId == request.UserId)
            .OrderByDescending(s => s.SavedDate)
            .Select(s => new SavedPostDto
            {
                PostId = s.PostId,
                AuthorId = s.Post.AuthorId,
                Content = s.Post.Content,
                SavedDate = s.SavedDate
            })
            .ToListAsync(cancellationToken);

        return Result<List<SavedPostDto>>.Success(savedPosts);
    }
}