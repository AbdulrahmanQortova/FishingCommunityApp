using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Community.Posts.Commands.ToggleSavePost;

public class ToggleSavePostCommand : IRequest<Result<bool>> // returns true if now saved, false if unsaved
{
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
}