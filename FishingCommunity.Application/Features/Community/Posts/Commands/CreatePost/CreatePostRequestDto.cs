// Features/Community/Posts/Commands/CreatePost/CreatePostRequestDto.cs
namespace FishingCommunity.Application.Features.Community.Posts.Commands.CreatePost;

public class CreatePostRequestDto
{
    public string Content { get; set; } = string.Empty;
    public List<string>? PhotoUrls { get; set; }
}