namespace FishingCommunity.Application.Features.Community.Posts.Queries.GetFeed;

public class PostSummaryDto
{
    public Guid Id { get; set; }
    public Guid AuthorId { get; set; }
    public string Content { get; set; } = string.Empty;
    public IReadOnlyList<string> PhotoUrls { get; set; } = new List<string>();
    public bool IsEdited { get; set; }
    public DateTime CreatedDate { get; set; }

    public int LikesCount { get; set; }
    public int DislikesCount { get; set; }
    public int CommentsCount { get; set; }

    // Only meaningful when RequestingUserId was provided — otherwise null.
    public string? CurrentUserReaction { get; set; }
    public bool IsSavedByCurrentUser { get; set; }
}