using FishingCommunity.Domain.Enums;

namespace FishingCommunity.Application.Features.Chat.Commands.SendMessage;

public class SendMessageRequestDto
{
    public Guid RecipientId { get; set; }
    public MessageType Type { get; set; } = MessageType.Text;
    public string? TextContent { get; set; }
    public string? MediaUrl { get; set; }
}