using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Payments.Queries.GetMyPayments;

public class GetMyPaymentsQuery : IRequest<Result<List<MyPaymentDto>>>
{
    public Guid UserId { get; set; }
}