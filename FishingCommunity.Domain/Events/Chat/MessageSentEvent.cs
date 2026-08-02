using FishingCommunity.Domain.Common;
using FishingCommunity.Domain.Enums;

namespace FishingCommunity.Domain.Events.Chat;

public class MessageSentEvent : DomainEvent
{
    public Guid ConversationId { get; }
    public Guid MessageId { get; }
    public Guid SenderId { get; }
    public Guid RecipientId { get; }
    public MessageType Type { get; }

    public MessageSentEvent(Guid conversationId, Guid messageId, Guid senderId, Guid recipientId, MessageType type)
    {
        ConversationId = conversationId;
        MessageId = messageId;
        SenderId = senderId;
        RecipientId = recipientId;
        Type = type;
    }
}