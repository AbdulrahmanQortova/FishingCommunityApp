using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Community.Posts.Commands.DeletePost;

public class DeletePostCommand : IRequest<Result>
{
    public Guid PostId { get; set; }
    public Guid RequestingUserId { get; set; }
}