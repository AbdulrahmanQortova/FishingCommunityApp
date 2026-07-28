namespace FishingCommunity.Application.Features.Community.Posts.Queries.GetPostDetails;

public class PostDetailsDto
{
    public Guid Id { get; set; }
    public Guid AuthorId { get; set; }
    public string Content { get; set; } = string.Empty;
    public IReadOnlyList<string> PhotoUrls { get; set; } = new List<string>();
    public bool IsEdited { get; set; }
    public DateTime CreatedDate { get; set; }
    public int LikesCount { get; set; }
    public int DislikesCount { get; set; }

    public List<CommentDto> Comments { get; set; } = new();
}

public class CommentDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid? ParentCommentId { get; set; }
    public bool IsEdited { get; set; }
    public bool IsRemoved { get; set; }
    public DateTime CreatedDate { get; set; }
}