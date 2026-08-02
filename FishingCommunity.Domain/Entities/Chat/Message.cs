using FishingCommunity.Domain.Common;
using FishingCommunity.Domain.Exceptions;

namespace FishingCommunity.Domain.Entities.Chat;

public class Message : BaseEntity
{
    public Guid ConversationId { get; private set; }
    public Conversation Conversation { get; private set; } = null!;

    public Guid SenderId { get; private set; }
    public Enums.MessageType Type { get; private set; }

    public string? TextContent { get; private set; }
    public string? MediaUrl { get; private set; } // For Image/Voice messages

    public bool IsRead { get; private set; }
    public DateTime? ReadDate { get; private set; }

    public DateTime CreatedDate { get; private set; }

    private Message() { } // EF Core

    internal Message(Guid conversationId, Guid senderId, Enums.MessageType type, string? textContent, string? mediaUrl)
    {
        if (type == Enums.MessageType.Text && string.IsNullOrWhiteSpace(textContent))
        {
            throw new BusinessRuleValidationException("Text message content cannot be empty.");
        }

        if (type is Enums.MessageType.Image or Enums.MessageType.Voice && string.IsNullOrWhiteSpace(mediaUrl))
        {
            throw new BusinessRuleValidationException("Media URL is required for image/voice messages.");
        }

        ConversationId = conversationId;
        SenderId = senderId;
        Type = type;
        TextContent = textContent;
        MediaUrl = mediaUrl;
        CreatedDate = DateTime.UtcNow;
    }

    public void MarkAsRead()
    {
        if (IsRead) return;

        IsRead = true;
        ReadDate = DateTime.UtcNow;
    }
}