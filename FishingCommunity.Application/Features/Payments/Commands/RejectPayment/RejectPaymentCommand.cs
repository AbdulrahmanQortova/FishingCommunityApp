using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Payments.Commands.RejectPayment;

public class RejectPaymentCommand : IRequest<Result>
{
    public Guid PaymentId { get; set; }
    public Guid AdminUserId { get; set; }
    public string Reason { get; set; } = string.Empty;
}