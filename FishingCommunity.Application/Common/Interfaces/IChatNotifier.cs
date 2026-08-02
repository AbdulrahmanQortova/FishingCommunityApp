using FishingCommunity.Domain.Enums;

namespace FishingCommunity.Application.Common.Interfaces;

public interface IChatNotifier
{
    Task NotifyMessageReceivedAsync(Guid recipientUserId, ChatMessageNotification message, CancellationToken cancellationToken = default);
    Task NotifyTypingAsync(Guid recipientUserId, Guid conversationId, Guid typingUserId, CancellationToken cancellationToken = default);
    Task NotifyMessageReadAsync(Guid recipientUserId, Guid conversationId, Guid readByUserId, CancellationToken cancellationToken = default);
    Task<bool> IsUserOnlineAsync(Guid userId, CancellationToken cancellationToken = default);
}

public class ChatMessageNotification
{
    public Guid ConversationId { get; set; }
    public Guid MessageId { get; set; }
    public Guid SenderId { get; set; }
    public MessageType Type { get; set; }
    public string? TextContent { get; set; }
    public string? MediaUrl { get; set; }
    public DateTime SentDate { get; set; }
}