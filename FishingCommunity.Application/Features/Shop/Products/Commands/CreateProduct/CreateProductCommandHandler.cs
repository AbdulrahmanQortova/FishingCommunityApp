using FishingCommunity.Domain.Entities.Shop;
using FishingCommunity.Domain.Exceptions;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Shop.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<CreateProductResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CreateProductResponse>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var store = await _unitOfWork.Repository<Store>().GetByIdAsync(request.StoreId, cancellationToken);

        if (store is null)
        {
            throw new NotFoundException(nameof(Store), request.StoreId);
        }

        if (store.OwnerId != request.RequestingUserId)
        {
            return Result<CreateProductResponse>.Failure("You can only add products to your own store.");
        }

        // Store.EnsureCanSell() throws BusinessRuleValidationException if the store
        // isn't Active yet (e.g. still UnderReview) — propagates to the middleware.
        store.EnsureCanSell();

        var categoryExists = await _unitOfWork.Repository<Category>().AnyAsync(c => c.Id == request.CategoryId, cancellationToken);

        if (!categoryExists)
        {
            throw new NotFoundException(nameof(Category), request.CategoryId);
        }

        var product = new Product(request.StoreId, request.CategoryId, request.Name, request.Price, request.StockQuantity, request.Description);

        await _unitOfWork.Repository<Product>().AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new CreateProductResponse
        {
            ProductId = product.Id,
            Name = product.Name,
            Price = product.Price
        };

        return Result<CreateProductResponse>.Success(response, "Product created successfully.");
    }
}