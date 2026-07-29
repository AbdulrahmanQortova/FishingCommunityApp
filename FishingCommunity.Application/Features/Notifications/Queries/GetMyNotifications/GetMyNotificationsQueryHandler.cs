using FishingCommunity.Application.Common.Extensions;
using FishingCommunity.Domain.Entities.Notifications;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Notifications.Queries.GetMyNotifications;

public class GetMyNotificationsQueryHandler : IRequestHandler<GetMyNotificationsQuery, Result<PaginatedList<NotificationDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetMyNotificationsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PaginatedList<NotificationDto>>> Handle(GetMyNotificationsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Notification>().Query()
            .Where(n => n.RecipientUserId == request.UserId);

        if (request.UnreadOnly)
        {
            query = query.Where(n => !n.IsRead);
        }

        var projectedQuery = query
            .OrderByDescending(n => n.CreatedDate)
            .Select(n => new NotificationDto
            {
                Id = n.Id,
                Type = n.Type,
                Title = n.Title,
                Message = n.Message,
                RelatedEntityId = n.RelatedEntityId,
                IsRead = n.IsRead,
                CreatedDate = n.CreatedDate
            });

        var result = await projectedQuery.ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);

        return Result<PaginatedList<NotificationDto>>.Success(result);
    }
}