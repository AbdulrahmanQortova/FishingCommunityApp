namespace FishingCommunity.Application.Features.Chat.Queries.GetMyConversations;

public class ConversationSummaryDto
{
    public Guid Id { get; set; }
    public Guid OtherParticipantId { get; set; }
    public string? LastMessagePreview { get; set; }
    public DateTime? LastMessageDate { get; set; }
    public int UnreadCount { get; set; }
}