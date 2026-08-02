using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Domain.Entities.Chat;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FishingCommunity.Application.Features.Chat.Commands.SendMessage;

public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, Result<SendMessageResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IChatNotifier _chatNotifier;

    public SendMessageCommandHandler(IUnitOfWork unitOfWork, IChatNotifier chatNotifier)
    {
        _unitOfWork = unitOfWork;
        _chatNotifier = chatNotifier;
    }

    public async Task<Result<SendMessageResponse>> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        // Find the existing conversation between these two users, if any — using the
        // same normalized ordering logic the Conversation constructor applies, so this
        // lookup works regardless of who's "sender" vs "recipient" this time around.
        var conversation = await _unitOfWork.Repository<Conversation>().Query()
            .Where(c =>
                (c.ParticipantOneId == request.SenderId && c.ParticipantTwoId == request.RecipientId) ||
                (c.ParticipantOneId == request.RecipientId && c.ParticipantTwoId == request.SenderId))
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(cancellationToken);

        if (conversation is null)
        {
            conversation = new Conversation(request.SenderId, request.RecipientId);
            await _unitOfWork.Repository<Conversation>().AddAsync(conversation, cancellationToken);
        }

        var message = conversation.AddMessage(request.SenderId, request.Type, request.TextContent, request.MediaUrl);

        _unitOfWork.Repository<Conversation>().Update(conversation);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Broadcast in real-time immediately after persisting — best-effort, doesn't
        // fail the whole send if the recipient happens to be offline or the hub
        // connection has an issue (the message is already safely stored either way).
        await _chatNotifier.NotifyMessageReceivedAsync(
            request.RecipientId,
            new ChatMessageNotification
            {
                ConversationId = conversation.Id,
                MessageId = message.Id,
                SenderId = request.SenderId,
                Type = request.Type,
                TextContent = request.TextContent,
                MediaUrl = request.MediaUrl,
                SentDate = message.CreatedDate
            },
            cancellationToken);

        var response = new SendMessageResponse
        {
            ConversationId = conversation.Id,
            MessageId = message.Id,
            SentDate = message.CreatedDate
        };

        return Result<SendMessageResponse>.Success(response);
    }
}