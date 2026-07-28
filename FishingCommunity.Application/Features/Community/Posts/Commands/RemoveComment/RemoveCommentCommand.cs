using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Community.Posts.Commands.RemoveComment;

public class RemoveCommentCommand : IRequest<Result>
{
    public Guid PostId { get; set; }
    public Guid CommentId { get; set; }
    public Guid RequestingUserId { get; set; }
}