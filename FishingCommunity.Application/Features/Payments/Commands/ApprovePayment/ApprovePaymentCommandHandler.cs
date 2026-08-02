using FishingCommunity.Domain.Entities.Payments;
using FishingCommunity.Domain.Exceptions;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Payments.Commands.ApprovePayment;

public class ApprovePaymentCommandHandler : IRequestHandler<ApprovePaymentCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public ApprovePaymentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ApprovePaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await _unitOfWork.Repository<Payment>().GetByIdAsync(request.PaymentId, cancellationToken);

        if (payment is null)
        {
            throw new NotFoundException(nameof(Payment), request.PaymentId);
        }

        payment.Approve(request.AdminUserId);

        _unitOfWork.Repository<Payment>().Update(payment);

        // Confirming the payment also confirms the order itself (moves it from
        // Pending to Confirmed), since the order shouldn't proceed to processing/shipping
        // until payment is verified.
        var order = await _unitOfWork.Repository<Domain.Entities.Shop.Order>().GetByIdAsync(payment.OrderId, cancellationToken);
        order?.Confirm();

        if (order is not null)
        {
            _unitOfWork.Repository<Domain.Entities.Shop.Order>().Update(order);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Payment approved successfully.");
    }
}