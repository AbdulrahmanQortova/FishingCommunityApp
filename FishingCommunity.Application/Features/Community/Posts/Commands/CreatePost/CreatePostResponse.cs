namespace FishingCommunity.Application.Features.Community.Posts.Commands.CreatePost;

public class CreatePostResponse
{
    public Guid PostId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}