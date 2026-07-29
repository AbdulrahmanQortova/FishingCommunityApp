using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Shop.Stores.Commands.ApproveStore;

public class ApproveStoreCommand : IRequest<Result>
{
    public Guid StoreId { get; set; }
}