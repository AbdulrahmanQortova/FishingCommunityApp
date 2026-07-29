using FishingCommunity.Application.Common.Extensions;
using FishingCommunity.Domain.Entities.Shop;
using FishingCommunity.Domain.Enums;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Shop.Products.Queries.GetProducts;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, Result<PaginatedList<ProductSummaryDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetProductsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PaginatedList<ProductSummaryDto>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Product>().Query()
            .Where(p => p.Status != ProductStatus.Discontinued);

        if (request.CategoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == request.CategoryId.Value);
        }

        if (request.StoreId.HasValue)
        {
            query = query.Where(p => p.StoreId == request.StoreId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(p => p.Name.Contains(request.SearchTerm));
        }

        if (request.MaxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= request.MaxPrice.Value);
        }

        var projectedQuery = query
            .OrderByDescending(p => p.CreatedDate)
            .Select(p => new ProductSummaryDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                MainPhotoUrl = p.MainPhotoUrl,
                AverageRating = p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : null,
                InStock = p.Status == ProductStatus.Active,
                StoreName = p.Store.Name
            });

        var result = await projectedQuery.ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);

        return Result<PaginatedList<ProductSummaryDto>>.Success(result);
    }
}