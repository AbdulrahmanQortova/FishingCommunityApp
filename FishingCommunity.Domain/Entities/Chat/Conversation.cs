using FishingCommunity.Domain.Common;
using FishingCommunity.Domain.Exceptions;

namespace FishingCommunity.Domain.Entities.Chat;

public class Conversation : BaseAuditableEntity, IAggregateRoot
{
    // Private (1-to-1) chat only, per current scope — always exactly two participants.
    public Guid ParticipantOneId { get; private set; }
    public Guid ParticipantTwoId { get; private set; }

    public DateTime? LastMessageDate { get; private set; }
    public string? LastMessagePreview { get; private set; }

    private readonly List<Message> _messages = new();
    public IReadOnlyCollection<Message> Messages => _messages.AsReadOnly();

    private Conversation() { } // EF Core

    public Conversation(Guid participantOneId, Guid participantTwoId)
    {
        if (participantOneId == participantTwoId)
        {
            throw new BusinessRuleValidationException("Cannot start a conversation with yourself.");
        }

        // Normalize participant order so a lookup for (A, B) always matches how (A, B)
        // or (B, A) was originally created — prevents duplicate conversations between
        // the same two users depending on who initiated it.
        if (participantOneId.CompareTo(participantTwoId) < 0)
        {
            ParticipantOneId = participantOneId;
            ParticipantTwoId = participantTwoId;
        }
        else
        {
            ParticipantOneId = participantTwoId;
            ParticipantTwoId = participantOneId;
        }
    }

    public bool HasParticipant(Guid userId) => ParticipantOneId == userId || ParticipantTwoId == userId;

    public Guid GetOtherParticipant(Guid userId)
    {
        if (!HasParticipant(userId))
        {
            throw new BusinessRuleValidationException("User is not a participant in this conversation.");
        }

        return ParticipantOneId == userId ? ParticipantTwoId : ParticipantOneId;
    }

    public Message AddMessage(Guid senderId, Enums.MessageType type, string? textContent, string? mediaUrl)
    {
        if (!HasParticipant(senderId))
        {
            throw new BusinessRuleValidationException("Sender is not a participant in this conversation.");
        }

        var message = new Message(Id, senderId, type, textContent, mediaUrl);
        _messages.Add(message);

        LastMessageDate = message.CreatedDate;
        LastMessagePreview = type switch
        {
            Enums.MessageType.Text => textContent?.Length > 100 ? textContent[..100] + "..." : textContent,
            Enums.MessageType.Image => "📷 Photo",
            Enums.MessageType.Voice => "🎤 Voice message",
            _ => "New message"
        };

        return message;
    }
}