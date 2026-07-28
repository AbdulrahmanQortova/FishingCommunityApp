using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Community.Posts.Queries.GetPostDetails;

public class GetPostDetailsQuery : IRequest<Result<PostDetailsDto>>
{
    public Guid PostId { get; set; }
}