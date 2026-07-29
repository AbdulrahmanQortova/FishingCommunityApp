using FishingCommunity.Domain.Entities.Shop;
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
        // 1. Load the cart with its items and the live Product for each item.
        var cart = await _unitOfWork.Repository<Carts>().Query()
            .Where(c => c.UserId == request.UserId)
            .Include(c => c.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(cancellationToken);

        if (cart is null || cart.Items.Count == 0)
        {
            return Result<CheckoutResponse>.Failure("Your cart is empty.");
        }

        // 2. Validate the shipping address belongs to this user.
        var addressExists = await _unitOfWork.Repository<ShippingAddress>()
            .AnyAsync(a => a.Id == request.ShippingAddressId && a.UserId == request.UserId, cancellationToken);

        if (!addressExists)
        {
            throw new NotFoundException(nameof(ShippingAddress), request.ShippingAddressId);
        }

        // 3. Re-validate stock and price against the LIVE product data (never trust the
        // cart's price snapshot for the final charge — prices/stock may have changed
        // since the item was added).
        var orderItemsData = new List<(Guid ProductId, string ProductName, int Quantity, decimal UnitPrice)>();

        foreach (var item in cart.Items)
        {
            if (item.Product.Status != Domain.Enums.ProductStatus.Active)
            {
                return Result<CheckoutResponse>.Failure($"\"{item.Product.Name}\" is no longer available.");
            }

            if (item.Quantity > item.Product.StockQuantity)
            {
                return Result<CheckoutResponse>.Failure($"Only {item.Product.StockQuantity} unit(s) of \"{item.Product.Name}\" are available.");
            }

            orderItemsData.Add((item.Product.Id, item.Product.Name, item.Quantity, item.Product.Price));
        }

        // 4. Reserve stock for each product (decrements StockQuantity, throws if insufficient
        // — defends against a race condition between the check above and this reservation).
        foreach (var item in cart.Items)
        {
            item.Product.ReserveStock(item.Quantity);
            _unitOfWork.Repository<Product>().Update(item.Product);
        }

        // 5. Create the order from the validated snapshot data.
        var order = Order.CreateFromCartItems(request.UserId, request.ShippingAddressId, orderItemsData);

        // 6. Apply coupon, if provided.
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

        // 7. Clear the cart now that its contents have become an order.
        cart.Clear();
        _unitOfWork.Repository<Carts>().Update(cart);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new CheckoutResponse
        {
            OrderId = order.Id,
            SubtotalAmount = order.SubtotalAmount,
            DiscountAmount = order.DiscountAmount,
            TotalAmount = order.TotalAmount
        };

        return Result<CheckoutResponse>.Success(response, "Order placed successfully.");
    }
}