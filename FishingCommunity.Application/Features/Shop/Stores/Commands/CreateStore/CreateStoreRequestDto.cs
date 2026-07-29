// Features/Shop/Stores/Commands/CreateStore/CreateStoreRequestDto.cs
namespace FishingCommunity.Application.Features.Shop.Stores.Commands.CreateStore;

public class CreateStoreRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}