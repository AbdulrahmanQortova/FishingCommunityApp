using FishingCommunity.Application.Common.Extensions;
using FishingCommunity.Domain.Entities.Chat;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Chat.Queries.GetMessages;

public class GetMessagesQueryHandler : IRequestHandler<GetMessagesQuery, Result<PaginatedList<MessageDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetMessagesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PaginatedList<MessageDto>>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
    {
        var conversationExists = await _unitOfWork.Repository<Conversation>().AnyAsync(
            c => c.Id == request.ConversationId &&
                 (c.ParticipantOneId == request.RequestingUserId || c.ParticipantTwoId == request.RequestingUserId),
            cancellationToken);

        if (!conversationExists)
        {
            return Result<PaginatedList<MessageDto>>.Failure("Conversation not found or you don't have access to it.");
        }

        var query = _unitOfWork.Repository<Message>().Query()
            .Where(m => m.ConversationId == request.ConversationId)
            .OrderByDescending(m => m.CreatedDate) // Most recent first — typical chat UX (reverse for display).
            .Select(m => new MessageDto
            {
                Id = m.Id,
                SenderId = m.SenderId,
                Type = m.Type,
                TextContent = m.TextContent,
                MediaUrl = m.MediaUrl,
                IsRead = m.IsRead,
                CreatedDate = m.CreatedDate
            });

        var result = await query.ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);

        return Result<PaginatedList<MessageDto>>.Success(result);
    }
}