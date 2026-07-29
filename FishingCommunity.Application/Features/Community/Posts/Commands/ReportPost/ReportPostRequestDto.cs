// Features/Community/Posts/Commands/ReportPost/ReportPostRequestDto.cs
using FishingCommunity.Domain.Enums;

namespace FishingCommunity.Application.Features.Community.Posts.Commands.ReportPost;

public class ReportPostRequestDto
{
    public ReportReason Reason { get; set; }
    public string? AdditionalDetails { get; set; }
}