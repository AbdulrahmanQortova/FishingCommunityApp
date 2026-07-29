using FishingCommunity.Domain.Entities.Shop;
using FishingCommunity.Domain.Exceptions;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Shop.Wishlist.Commands.ToggleWishlistItem;

public class ToggleWishlistItemCommandHandler : IRequestHandler<ToggleWishlistItemCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ToggleWishlistItemCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(ToggleWishlistItemCommand request, CancellationToken cancellationToken)
    {
        var productExists = await _unitOfWork.Repository<Product>().AnyAsync(p => p.Id == request.ProductId, cancellationToken);

        if (!productExists)
        {
            throw new NotFoundException(nameof(Product), request.ProductId);
        }

        var existing = (await _unitOfWork.Repository<WishlistItem>()
            .FindAsync(w => w.UserId == request.UserId && w.ProductId == request.ProductId, cancellationToken))
            .FirstOrDefault();

        if (existing is not null)
        {
            _unitOfWork.Repository<WishlistItem>().Remove(existing);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(false, "Removed from wishlist.");
        }

        var wishlistItem = new WishlistItem(request.UserId, request.ProductId);
        await _unitOfWork.Repository<WishlistItem>().AddAsync(wishlistItem, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "Added to wishlist.");
    }
}