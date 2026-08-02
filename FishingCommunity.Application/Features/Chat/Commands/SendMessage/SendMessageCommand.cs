using FishingCommunity.Domain.Enums;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Chat.Commands.SendMessage;

public class SendMessageCommand : IRequest<Result<SendMessageResponse>>
{
    public Guid SenderId { get; set; }
    public Guid RecipientId { get; set; }
    public MessageType Type { get; set; } = MessageType.Text;
    public string? TextContent { get; set; }
    public string? MediaUrl { get; set; }
}