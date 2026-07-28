using FishingCommunity.Domain.Enums;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Community.Posts.Commands.ReactToPost;

public class ReactToPostCommand : IRequest<Result>
{
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public ReactionType Type { get; set; }
}