using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Admin.Stores.Commands.SuspendStore;

public class SuspendStoreCommand : IRequest<Result>
{
    public Guid StoreId { get; set; }
}