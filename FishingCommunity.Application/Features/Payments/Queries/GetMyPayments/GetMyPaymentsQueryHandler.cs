using FishingCommunity.Domain.Entities.Payments;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FishingCommunity.Application.Features.Payments.Queries.GetMyPayments;

public class GetMyPaymentsQueryHandler : IRequestHandler<GetMyPaymentsQuery, Result<List<MyPaymentDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetMyPaymentsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<MyPaymentDto>>> Handle(GetMyPaymentsQuery request, CancellationToken cancellationToken)
    {
        var payments = await _unitOfWork.Repository<Payment>().Query()
            .Where(p => p.UserId == request.UserId)
            .OrderByDescending(p => p.CreatedDate)
            .Select(p => new MyPaymentDto
            {
                Id = p.Id,
                OrderId = p.OrderId,
                Method = p.Method,
                Status = p.Status,
                Amount = p.Amount,
                RejectionReason = p.RejectionReason,
                CreatedDate = p.CreatedDate
            })
            .ToListAsync(cancellationToken);

        return Result<List<MyPaymentDto>>.Success(payments);
    }
}