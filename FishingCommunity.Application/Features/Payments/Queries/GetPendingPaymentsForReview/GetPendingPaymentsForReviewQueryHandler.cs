using FishingCommunity.Application.Common.Extensions;
using FishingCommunity.Domain.Entities.Payments;
using FishingCommunity.Domain.Enums;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Payments.Queries.GetPendingPaymentsForReview;

public class GetPendingPaymentsForReviewQueryHandler : IRequestHandler<GetPendingPaymentsForReviewQuery, Result<PaginatedList<PendingPaymentDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPendingPaymentsForReviewQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PaginatedList<PendingPaymentDto>>> Handle(GetPendingPaymentsForReviewQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Payment>().Query()
            .Where(p => p.Status == PaymentStatus.UnderReview)
            .OrderBy(p => p.ProofSubmittedDate) // Oldest first — fair review queue (FIFO).
            .Select(p => new PendingPaymentDto
            {
                Id = p.Id,
                OrderId = p.OrderId,
                UserId = p.UserId,
                Method = p.Method,
                Amount = p.Amount,
                SenderPhoneOrHandle = p.SenderPhoneOrHandle,
                TransferProofUrl = p.TransferProofUrl,
                ProofSubmittedDate = p.ProofSubmittedDate
            });

        var result = await query.ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);

        return Result<PaginatedList<PendingPaymentDto>>.Success(result);
    }
}