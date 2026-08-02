using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Chat.Commands.MarkMessagesAsRead;

public class MarkMessagesAsReadCommand : IRequest<Result>
{
    public Guid ConversationId { get; set; }
    public Guid UserId { get; set; } // The reader
}