using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Chat.Queries.GetMyConversations;

public class GetMyConversationsQuery : IRequest<Result<List<ConversationSummaryDto>>>
{
    public Guid UserId { get; set; }
}