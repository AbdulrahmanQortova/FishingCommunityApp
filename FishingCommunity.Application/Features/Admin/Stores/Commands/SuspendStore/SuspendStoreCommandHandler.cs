using FishingCommunity.Domain.Entities.Shop;
using FishingCommunity.Domain.Exceptions;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Admin.Stores.Commands.SuspendStore;

public class SuspendStoreCommandHandler : IRequestHandler<SuspendStoreCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public SuspendStoreCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(SuspendStoreCommand request, CancellationToken cancellationToken)
    {
        var store = await _unitOfWork.Repository<Store>().GetByIdAsync(request.StoreId, cancellationToken);

        if (store is null)
        {
            throw new NotFoundException(nameof(Store), request.StoreId);
        }

        store.Suspend();

        _unitOfWork.Repository<Store>().Update(store);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Store suspended successfully.");
    }
}