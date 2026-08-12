namespace FishingHub.Mobile.Models.Api.Community;

public class PostDetailsDto
{
    public Guid Id { get; set; }
    public Guid AuthorId { get; set; }
    public string Content { get; set; } = string.Empty;
    public List<string> PhotoUrls { get; set; } = new();
    public bool IsEdited { get; set; }
    public DateTime CreatedDate { get; set; }
    public int LikesCount { get; set; }
    public int DislikesCount { get; set; }
    public List<CommentDto> Comments { get; set; } = new();
}