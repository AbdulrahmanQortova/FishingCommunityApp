using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Shop.Stores.Commands.CreateStore;

public class CreateStoreCommand : IRequest<Result<CreateStoreResponse>>
{
    public Guid OwnerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}