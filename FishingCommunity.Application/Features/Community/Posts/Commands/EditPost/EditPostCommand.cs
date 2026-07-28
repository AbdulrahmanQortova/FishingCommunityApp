using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Community.Posts.Commands.EditPost;

public class EditPostCommand : IRequest<Result>
{
    public Guid PostId { get; set; }
    public Guid RequestingUserId { get; set; }
    public string Content { get; set; } = string.Empty;
}