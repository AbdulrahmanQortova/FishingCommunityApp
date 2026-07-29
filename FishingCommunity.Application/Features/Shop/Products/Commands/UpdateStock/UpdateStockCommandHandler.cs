using FishingCommunity.Domain.Entities.Shop;
using FishingCommunity.Domain.Exceptions;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FishingCommunity.Application.Features.Shop.Products.Commands.UpdateStock;

public class UpdateStockCommandHandler : IRequestHandler<UpdateStockCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateStockCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateStockCommand request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Repository<Product>().Query()
            .Where(p => p.Id == request.ProductId)
            .Include(p => p.Store)
            .FirstOrDefaultAsync(cancellationToken);

        if (product is null)
        {
            throw new NotFoundException(nameof(Product), request.ProductId);
        }

        if (product.Store.OwnerId != request.RequestingUserId)
        {
            return Result.Failure("You are not authorized to update stock for this product.");
        }

        product.IncreaseStock(request.QuantityToAdd);

        _unitOfWork.Repository<Product>().Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success($"Stock updated. New quantity: {product.StockQuantity}.");
    }
}