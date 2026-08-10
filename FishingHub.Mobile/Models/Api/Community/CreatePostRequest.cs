namespace FishingHub.Mobile.Models.Api.Community;

public class CreatePostRequest
{
    public string Content { get; set; } = string.Empty;
    public List<string>? PhotoUrls { get; set; }
}