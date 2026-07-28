using FishingCommunity.Application.Features.Trips.Boats.Commands.CreateBoat;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Community.Posts.Commands.CreatePost;

public class CreatePostCommand : IRequest<Result<CreatePostResponse>>
{
    public Guid AuthorId { get; set; }
    public string Content { get; set; } = string.Empty;
    public List<string>? PhotoUrls { get; set; }
}