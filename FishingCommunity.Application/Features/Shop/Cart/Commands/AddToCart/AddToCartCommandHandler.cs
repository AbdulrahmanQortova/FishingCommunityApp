using FishingCommunity.Domain.Entities.Shop;
using FishingCommunity.Domain.Exceptions;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FishingCommunity.Application.Features.Shop.Cart.Commands.AddToCart;

public class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddToCartCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(AddToCartCommand request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Repository<Product>().GetByIdAsync(request.ProductId, cancellationToken);

        if (product is null)
        {
            throw new NotFoundException(nameof(Product), request.ProductId);
        }

        if (product.Status != Domain.Enums.ProductStatus.Active)
        {
            return Result.Failure("This product is currently unavailable.");
        }

        if (request.Quantity > product.StockQuantity)
        {
            return Result.Failure($"Only {product.StockQuantity} units are available.");
        }

        var cart = await _unitOfWork.Repository<Domain.Entities.Shop.Cart>().Query()
            .Where(c => c.UserId == request.UserId)
            .Include(c => c.Items)
            .FirstOrDefaultAsync(cancellationToken);

        if (cart is null)
        {
            cart = new Domain.Entities.Shop.Cart(request.UserId);
            await _unitOfWork.Repository<Domain.Entities.Shop.Cart>().AddAsync(cart, cancellationToken);
        }

        cart.AddItem(request.ProductId, request.Quantity, product.Price);

        _unitOfWork.Repository<Domain.Entities.Shop.Cart>().Update(cart);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Item added to cart.");
    }
}