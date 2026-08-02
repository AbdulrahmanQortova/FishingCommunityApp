using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Payments.Queries.GetPendingPaymentsForReview;

public class GetPendingPaymentsForReviewQuery : IRequest<Result<PaginatedList<PendingPaymentDto>>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}