using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Payments.Commands.ApprovePayment;

public class ApprovePaymentCommand : IRequest<Result>
{
    public Guid PaymentId { get; set; }
    public Guid AdminUserId { get; set; }
}