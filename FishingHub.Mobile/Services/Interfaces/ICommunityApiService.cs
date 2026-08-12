using FishingHub.Mobile.Models.Api;
using FishingHub.Mobile.Models.Api.Community;

namespace FishingHub.Mobile.Services.Interfaces;

public interface ICommunityApiService
{
    Task<ApiResult<PaginatedResult<PostSummary>>> GetFeedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<ApiResult<object>> CreatePostAsync(CreatePostRequest request, CancellationToken cancellationToken = default);
    Task<ApiResult<bool>> ReactToPostAsync(Guid postId, string reactionType, CancellationToken cancellationToken = default);
    Task<ApiResult<PostDetailsDto>> GetPostDetailsAsync(Guid postId, CancellationToken cancellationToken = default);
    Task<ApiResult<object>> AddCommentAsync(Guid postId, string content, Guid? parentCommentId, CancellationToken cancellationToken = default);
}