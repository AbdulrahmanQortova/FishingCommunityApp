using FishingCommunity.Domain.Enums;

namespace FishingCommunity.Application.Features.Chat.Queries.GetMessages;

public class MessageDto
{
    public Guid Id { get; set; }
    public Guid SenderId { get; set; }
    public MessageType Type { get; set; }
    public string? TextContent { get; set; }
    public string? MediaUrl { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedDate { get; set; }
}