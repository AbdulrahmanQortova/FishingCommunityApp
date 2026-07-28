using FishingCommunity.Domain.Enums;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Community.Posts.Commands.ReportPost;

public class ReportPostCommand : IRequest<Result>
{
    public Guid PostId { get; set; }
    public Guid ReportedByUserId { get; set; }
    public ReportReason Reason { get; set; }
    public string? AdditionalDetails { get; set; }
}