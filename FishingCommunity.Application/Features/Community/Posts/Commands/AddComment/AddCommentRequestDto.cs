// Features/Community/Posts/Commands/AddComment/AddCommentRequestDto.cs
namespace FishingCommunity.Application.Features.Community.Posts.Commands.AddComment;

public class AddCommentRequestDto
{
    public string Content { get; set; } = string.Empty;
    public Guid? ParentCommentId { get; set; }
}