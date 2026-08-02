using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Domain.Entities.Chat;
using FishingCommunity.Domain.Exceptions;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FishingCommunity.Application.Features.Chat.Commands.MarkMessagesAsRead;

public class MarkMessagesAsReadCommandHandler : IRequestHandler<MarkMessagesAsReadCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IChatNotifier _chatNotifier;

    public MarkMessagesAsReadCommandHandler(IUnitOfWork unitOfWork, IChatNotifier chatNotifier)
    {
        _unitOfWork = unitOfWork;
        _chatNotifier = chatNotifier;
    }

    public async Task<Result> Handle(MarkMessagesAsReadCommand request, CancellationToken cancellationToken)
    {
        var conversation = await _unitOfWork.Repository<Conversation>().Query()
            .Where(c => c.Id == request.ConversationId)
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(cancellationToken);

        if (conversation is null)
        {
            throw new NotFoundException(nameof(Conversation), request.ConversationId);
        }

        if (!conversation.HasParticipant(request.UserId))
        {
            return Result.Failure("You are not a participant in this conversation.");
        }

        // Mark all unread messages sent BY THE OTHER PERSON as read (a user doesn't
        // "read" their own messages).
        var unreadMessages = conversation.Messages
            .Where(m => m.SenderId != request.UserId && !m.IsRead)
            .ToList();

        foreach (var message in unreadMessages)
        {
            message.MarkAsRead();
        }

        _unitOfWork.Repository<Conversation>().Update(conversation);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (unreadMessages.Count > 0)
        {
            var otherParticipant = conversation.GetOtherParticipant(request.UserId);
            await _chatNotifier.NotifyMessageReadAsync(otherParticipant, conversation.Id, request.UserId, cancellationToken);
        }

        return Result.Success();
    }
}