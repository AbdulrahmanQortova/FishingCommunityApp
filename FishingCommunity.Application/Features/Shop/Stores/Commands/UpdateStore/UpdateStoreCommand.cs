using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Shop.Stores.Commands.UpdateStore;

public class UpdateStoreCommand : IRequest<Result>
{
    public Guid StoreId { get; set; }
    public Guid RequestingUserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}