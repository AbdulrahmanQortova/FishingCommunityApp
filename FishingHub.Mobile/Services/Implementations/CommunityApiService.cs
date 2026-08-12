using FishingHub.Mobile.Models.Api;
using FishingHub.Mobile.Models.Api.Community;
using FishingHub.Mobile.Services.Interfaces;

namespace FishingHub.Mobile.Services.Implementations;

public class CommunityApiService : ICommunityApiService
{
    private readonly IApiClient _apiClient;

    public CommunityApiService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<ApiResult<PaginatedResult<PostSummary>>> GetFeedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        return _apiClient.GetAsync<PaginatedResult<PostSummary>>(
            $"api/v1/posts?pageNumber={pageNumber}&pageSize={pageSize}",
            cancellationToken);
    }

    public Task<ApiResult<object>> CreatePostAsync(CreatePostRequest request, CancellationToken cancellationToken = default)
    {
        return _apiClient.PostAsync<CreatePostRequest, object>("api/v1/posts", request, cancellationToken);
    }

    public Task<ApiResult<bool>> ReactToPostAsync(Guid postId, string reactionType, CancellationToken cancellationToken = default)
    {
        return _apiClient.PostAsync<object, bool>($"api/v1/posts/{postId}/react", new { type = reactionType }, cancellationToken);
    }

    public Task<ApiResult<PostDetailsDto>> GetPostDetailsAsync(Guid postId, CancellationToken cancellationToken = default)
    {
        return _apiClient.GetAsync<PostDetailsDto>($"api/v1/posts/{postId}", cancellationToken);
    }

    public Task<ApiResult<object>> AddCommentAsync(Guid postId, string content, Guid? parentCommentId, CancellationToken cancellationToken = default)
    {
        return _apiClient.PostAsync<object, object>(
            $"api/v1/posts/{postId}/comments",
            new { content, parentCommentId },
            cancellationToken);
    }
}