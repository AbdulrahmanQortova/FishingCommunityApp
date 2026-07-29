namespace FishingCommunity.Application.Features.Shop.Stores.Commands.CreateStore;

public class CreateStoreResponse
{
    public Guid StoreId { get; set; }
    public string Name { get; set; } = string.Empty;
}