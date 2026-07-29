using FishingCommunity.Domain.Entities.Shop;
using FishingCommunity.Domain.Exceptions;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FishingCommunity.Application.Features.Shop.Orders.Commands.CancelOrder;

public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public CancelOrderCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Repository<Order>().Query()
            .Where(o => o.Id == request.OrderId)
            .Include(o => o.Items)
            .FirstOrDefaultAsync(cancellationToken);

        if (order is null)
        {
            throw new NotFoundException(nameof(Order), request.OrderId);
        }

        if (order.UserId != request.RequestingUserId)
        {
            return Result.Failure("You are not authorized to cancel this order.");
        }

        // Order.Cancel() throws BusinessRuleValidationException if it's already
        // delivered/cancelled/refunded — propagates to the middleware.
        order.Cancel(request.Reason);

        // Restore stock for each item since the order is no longer going through.
        foreach (var item in order.Items)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.ProductId, cancellationToken);
            product?.RestoreStock(item.Quantity);

            if (product is not null)
            {
                _unitOfWork.Repository<Product>().Update(product);
            }
        }

        _unitOfWork.Repository<Order>().Update(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Order cancelled successfully.");
    }
}