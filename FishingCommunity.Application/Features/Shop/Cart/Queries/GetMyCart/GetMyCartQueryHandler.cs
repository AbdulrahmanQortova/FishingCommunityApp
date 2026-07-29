using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Carts = FishingCommunity.Domain.Entities.Shop.Cart;

namespace FishingCommunity.Application.Features.Shop.Cart.Queries.GetMyCart;

public class GetMyCartQueryHandler : IRequestHandler<GetMyCartQuery, Result<CartDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetMyCartQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CartDto>> Handle(GetMyCartQuery request, CancellationToken cancellationToken)
    {
        var cart = await _unitOfWork.Repository<Carts>().Query()
            .Where(c => c.UserId == request.UserId)
            .Select(c => new CartDto
            {
                Items = c.Items.Select(i => new CartItemDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product.Name,
                    MainPhotoUrl = i.Product.MainPhotoUrl,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPriceSnapshot,
                    AvailableStock = i.Product.StockQuantity
                }).ToList(),
                Total = c.Items.Sum(i => i.Quantity * i.UnitPriceSnapshot)
            })
            .FirstOrDefaultAsync(cancellationToken);

        // No cart yet is not an error — just an empty cart.
        return Result<CartDto>.Success(cart ?? new CartDto());
    }
}