using FishingCommunity.Application.Common.Extensions;
using FishingCommunity.Domain.Entities.Shop;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Shop.Orders.Queries.GetMyOrders;

public class GetMyOrdersQueryHandler : IRequestHandler<GetMyOrdersQuery, Result<PaginatedList<MyOrderDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetMyOrdersQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PaginatedList<MyOrderDto>>> Handle(GetMyOrdersQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Order>().Query()
            .Where(o => o.UserId == request.UserId)
            .OrderByDescending(o => o.CreatedDate)
            .Select(o => new MyOrderDto
            {
                Id = o.Id,
                Status = o.Status,
                TotalAmount = o.TotalAmount,
                ItemsCount = o.Items.Count,
                CreatedDate = o.CreatedDate
            });

        var result = await query.ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);

        return Result<PaginatedList<MyOrderDto>>.Success(result);
    }
}