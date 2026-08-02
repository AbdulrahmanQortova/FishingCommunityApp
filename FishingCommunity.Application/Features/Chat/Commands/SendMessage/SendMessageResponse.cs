namespace FishingCommunity.Application.Features.Chat.Commands.SendMessage;

public class SendMessageResponse
{
    public Guid ConversationId { get; set; }
    public Guid MessageId { get; set; }
    public DateTime SentDate { get; set; }
}