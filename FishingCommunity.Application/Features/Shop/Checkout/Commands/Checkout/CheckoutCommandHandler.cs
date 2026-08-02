using FishingCommunity.Domain.Entities.Payments;
using FishingCommunity.Domain.Entities.Shop;
using FishingCommunity.Domain.Enums;
using FishingCommunity.Domain.Exceptions;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Carts = FishingCommunity.Domain.Entities.Shop.Cart;

namespace FishingCommunity.Application.Features.Shop.Checkout.Commands.Checkout;

public class CheckoutCommandHandler : IRequestHandler<CheckoutCommand, Result<CheckoutResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CheckoutCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CheckoutResponse>> Handle(CheckoutCommand request, CancellationToken cancellationToken)
    {
        var cart = await _unitOfWork.Repository<Carts>().Query()
            .Where(c => c.UserId == request.UserId)
            .Include(c => c.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(cancellationToken);

        if (cart is null || cart.Items.Count == 0)
        {
            return Result<CheckoutResponse>.Failure("Your cart is empty.");
        }

        var addressExists = await _unitOfWork.Repository<ShippingAddress>()
            .AnyAsync(a => a.Id == request.ShippingAddressId && a.UserId == request.UserId, cancellationToken);

        if (!addressExists)
        {
            throw new NotFoundException(nameof(ShippingAddress), request.ShippingAddressId);
        }

        var orderItemsData = new List<(Guid ProductId, string ProductName, int Quantity, decimal UnitPrice)>();

        foreach (var item in cart.Items)
        {
            if (item.Product.Status != ProductStatus.Active)
            {
                return Result<CheckoutResponse>.Failure($"\"{item.Product.Name}\" is no longer available.");
            }

            if (item.Quantity > item.Product.StockQuantity)
            {
                return Result<CheckoutResponse>.Failure($"Only {item.Product.StockQuantity} unit(s) of \"{item.Product.Name}\" are available.");
            }

            orderItemsData.Add((item.Product.Id, item.Product.Name, item.Quantity, item.Product.Price));
        }

        foreach (var item in cart.Items)
        {
            item.Product.ReserveStock(item.Quantity);
            _unitOfWork.Repository<Product>().Update(item.Product);
        }

        var order = Order.CreateFromCartItems(request.UserId, request.ShippingAddressId, orderItemsData);

        if (!string.IsNullOrWhiteSpace(request.CouponCode))
        {
            var coupon = (await _unitOfWork.Repository<Coupon>()
                .FindAsync(c => c.Code == request.CouponCode.ToUpperInvariant(), cancellationToken))
                .FirstOrDefault();

            if (coupon is null)
            {
                return Result<CheckoutResponse>.Failure("Invalid coupon code.");
            }

            if (!coupon.IsValidForUse(order.SubtotalAmount))
            {
                return Result<CheckoutResponse>.Failure("This coupon is not valid for this order.");
            }

            var discount = coupon.CalculateDiscount(order.SubtotalAmount);
            order.ApplyCoupon(coupon.Code, discount);
            coupon.RecordUsage();

            _unitOfWork.Repository<Coupon>().Update(coupon);
        }

        await _unitOfWork.Repository<Order>().AddAsync(order, cancellationToken);

        // Create the Payment record alongside the Order, in the same transaction.
        var payment = request.PaymentMethod == PaymentMethod.CashOnDelivery
            ? Payment.CreateCashOnDelivery(order.Id, request.UserId, order.TotalAmount)
            : Payment.CreateManualTransfer(order.Id, request.UserId, order.TotalAmount, request.PaymentMethod);

        await _unitOfWork.Repository<Payment>().AddAsync(payment, cancellationToken);

        cart.Clear();
        _unitOfWork.Repository<Carts>().Update(cart);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new CheckoutResponse
        {
            OrderId = order.Id,
            PaymentId = payment.Id,
            PaymentMethod = payment.Method,
            PaymentStatus = payment.Status,
            SubtotalAmount = order.SubtotalAmount,
            DiscountAmount = order.DiscountAmount,
            TotalAmount = order.TotalAmount
        };

        return Result<CheckoutResponse>.Success(response, "Order placed successfully.");
    }
}