namespace FishingCommunity.Application.Features.Community.Posts.Commands.AddComment;

public class AddCommentResponse
{
    public Guid CommentId { get; set; }
    public Guid PostId { get; set; }
    public string Content { get; set; } = string.Empty;
}