// Features/Community/Posts/Commands/ReactToPost/ReactToPostRequestDto.cs
using FishingCommunity.Domain.Enums;

namespace FishingCommunity.Application.Features.Community.Posts.Commands.ReactToPost;

public class ReactToPostRequestDto
{
    public ReactionType Type { get; set; }
}