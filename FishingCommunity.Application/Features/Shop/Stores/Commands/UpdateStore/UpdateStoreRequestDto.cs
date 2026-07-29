// Features/Shop/Stores/Commands/UpdateStore/UpdateStoreRequestDto.cs
namespace FishingCommunity.Application.Features.Shop.Stores.Commands.UpdateStore;

public class UpdateStoreRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}