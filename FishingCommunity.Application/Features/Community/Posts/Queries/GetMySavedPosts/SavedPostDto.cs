namespace FishingCommunity.Application.Features.Community.Posts.Queries.GetMySavedPosts;

public class SavedPostDto
{
    public Guid PostId { get; set; }
    public Guid AuthorId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime SavedDate { get; set; }
}