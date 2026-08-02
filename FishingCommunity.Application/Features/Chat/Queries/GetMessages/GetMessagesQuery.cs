using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Chat.Queries.GetMessages;

public class GetMessagesQuery : IRequest<Result<PaginatedList<MessageDto>>>
{
    public Guid ConversationId { get; set; }
    public Guid RequestingUserId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 30;
}