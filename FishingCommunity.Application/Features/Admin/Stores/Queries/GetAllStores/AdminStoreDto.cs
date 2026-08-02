using FishingCommunity.Domain.Enums;

namespace FishingCommunity.Application.Features.Admin.Stores.Queries.GetAllStores;

public class AdminStoreDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid OwnerId { get; set; }
    public StoreStatus Status { get; set; }
    public int ProductsCount { get; set; }
    public DateTime CreatedDate { get; set; }
}