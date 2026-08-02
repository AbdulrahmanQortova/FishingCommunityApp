using FishingCommunity.Domain.Entities.Payments;
using FishingCommunity.Domain.Exceptions;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Payments.Commands.RejectPayment;

public class RejectPaymentCommandHandler : IRequestHandler<RejectPaymentCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public RejectPaymentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RejectPaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await _unitOfWork.Repository<Payment>().GetByIdAsync(request.PaymentId, cancellationToken);

        if (payment is null)
        {
            throw new NotFoundException(nameof(Payment), request.PaymentId);
        }

        payment.Reject(request.AdminUserId, request.Reason);
        _unitOfWork.Repository<Payment>().Update(payment);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Payment rejected.");
    }
}