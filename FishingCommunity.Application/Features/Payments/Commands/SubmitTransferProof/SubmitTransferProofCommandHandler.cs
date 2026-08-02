using FishingCommunity.Domain.Entities.Payments;
using FishingCommunity.Domain.Exceptions;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Payments.Commands.SubmitTransferProof;

public class SubmitTransferProofCommandHandler : IRequestHandler<SubmitTransferProofCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public SubmitTransferProofCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(SubmitTransferProofCommand request, CancellationToken cancellationToken)
    {
        var payment = await _unitOfWork.Repository<Payment>().GetByIdAsync(request.PaymentId, cancellationToken);

        if (payment is null)
        {
            throw new NotFoundException(nameof(Payment), request.PaymentId);
        }

        if (payment.UserId != request.RequestingUserId)
        {
            return Result.Failure("You are not authorized to submit proof for this payment.");
        }

        payment.SubmitTransferProof(request.SenderPhoneOrHandle, request.ProofUrl);

        _unitOfWork.Repository<Payment>().Update(payment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Transfer proof submitted. Your payment is now under review.");
    }
}