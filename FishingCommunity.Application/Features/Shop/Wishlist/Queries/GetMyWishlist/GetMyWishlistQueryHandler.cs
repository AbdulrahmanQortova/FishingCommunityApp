using FishingCommunity.Domain.Entities.Shop;
using FishingCommunity.Domain.Enums;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FishingCommunity.Application.Features.Shop.Wishlist.Queries.GetMyWishlist;

public class GetMyWishlistQueryHandler : IRequestHandler<GetMyWishlistQuery, Result<List<WishlistItemDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetMyWishlistQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<WishlistItemDto>>> Handle(GetMyWishlistQuery request, CancellationToken cancellationToken)
    {
        var items = await _unitOfWork.Repository<WishlistItem>().Query()
            .Where(w => w.UserId == request.UserId)
            .Select(w => new WishlistItemDto
            {
                ProductId = w.ProductId,
                ProductName = w.Product.Name,
                Price = w.Product.Price,
                MainPhotoUrl = w.Product.MainPhotoUrl,
                InStock = w.Product.Status == ProductStatus.Active
            })
            .ToListAsync(cancellationToken);

        return Result<List<WishlistItemDto>>.Success(items);
    }
}