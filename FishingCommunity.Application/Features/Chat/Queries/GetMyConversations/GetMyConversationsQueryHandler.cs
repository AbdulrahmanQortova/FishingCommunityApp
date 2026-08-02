using FishingCommunity.Domain.Entities.Chat;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FishingCommunity.Application.Features.Chat.Queries.GetMyConversations;

public class GetMyConversationsQueryHandler : IRequestHandler<GetMyConversationsQuery, Result<List<ConversationSummaryDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetMyConversationsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<ConversationSummaryDto>>> Handle(GetMyConversationsQuery request, CancellationToken cancellationToken)
    {
        var conversations = await _unitOfWork.Repository<Conversation>().Query()
            .Where(c => c.ParticipantOneId == request.UserId || c.ParticipantTwoId == request.UserId)
            .OrderByDescending(c => c.LastMessageDate)
            .Select(c => new ConversationSummaryDto
            {
                Id = c.Id,
                OtherParticipantId = c.ParticipantOneId == request.UserId ? c.ParticipantTwoId : c.ParticipantOneId,
                LastMessagePreview = c.LastMessagePreview,
                LastMessageDate = c.LastMessageDate,
                UnreadCount = c.Messages.Count(m => m.SenderId != request.UserId && !m.IsRead)
            })
            .ToListAsync(cancellationToken);

        return Result<List<ConversationSummaryDto>>.Success(conversations);
    }
}