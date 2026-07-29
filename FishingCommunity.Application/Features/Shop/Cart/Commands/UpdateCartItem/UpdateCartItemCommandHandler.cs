using FishingCommunity.Domain.Exceptions;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Carts = FishingCommunity.Domain.Entities.Shop.Cart;

namespace FishingCommunity.Application.Features.Shop.Cart.Commands.UpdateCartItem;

public class UpdateCartItemCommandHandler : IRequestHandler<UpdateCartItemCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCartItemCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateCartItemCommand request, CancellationToken cancellationToken)
    {
        var cart = await _unitOfWork.Repository<Carts>().Query()
            .Where(c => c.UserId == request.UserId)
            .Include(c => c.Items)
            .FirstOrDefaultAsync(cancellationToken);

        if (cart is null)
        {
            throw new NotFoundException(nameof(Cart), request.UserId);
        }

        cart.UpdateItemQuantity(request.ProductId, request.Quantity);

        _unitOfWork.Repository<Carts>().Update(cart);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Cart updated.");
    }
}