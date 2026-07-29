using FishingCommunity.Domain.Entities.Shop;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FishingCommunity.Application.Features.Shop.Orders.Queries.GetOrderDetails;

public class GetOrderDetailsQueryHandler : IRequestHandler<GetOrderDetailsQuery, Result<OrderDetailsDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetOrderDetailsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<OrderDetailsDto>> Handle(GetOrderDetailsQuery request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Repository<Order>().Query()
            .Where(o => o.Id == request.OrderId)
            .Include(o => o.Items)
            .Include(o => o.ShippingAddress)
            .FirstOrDefaultAsync(cancellationToken);

        if (order is null)
        {
            return Result<OrderDetailsDto>.Failure("Order not found.");
        }

        if (order.UserId != request.RequestingUserId)
        {
            return Result<OrderDetailsDto>.Failure("You are not authorized to view this order.");
        }

        var dto = new OrderDetailsDto
        {
            Id = order.Id,
            Status = order.Status,
            SubtotalAmount = order.SubtotalAmount,
            DiscountAmount = order.DiscountAmount,
            TotalAmount = order.TotalAmount,
            CouponCode = order.CouponCode,
            CreatedDate = order.CreatedDate,
            ShippedDate = order.ShippedDate,
            DeliveredDate = order.DeliveredDate,
            ShippingFullName = order.ShippingAddress.FullName,
            ShippingAddressLine = order.ShippingAddress.AddressLine1,
            ShippingCity = order.ShippingAddress.City,
            Items = order.Items.Select(i => new OrderItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                LineTotal = i.LineTotal
            }).ToList()
        };

        return Result<OrderDetailsDto>.Success(dto);
    }
}