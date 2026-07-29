using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Shop.ShippingAddresses.Queries.GetMyAddresses;

public class GetMyAddressesQuery : IRequest<Result<List<ShippingAddressDto>>>
{
    public Guid UserId { get; set; }
}