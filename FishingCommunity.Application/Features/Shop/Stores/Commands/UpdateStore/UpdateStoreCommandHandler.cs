using FishingCommunity.Domain.Entities.Shop;
using FishingCommunity.Domain.Exceptions;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Shop.Stores.Commands.UpdateStore;

public class UpdateStoreCommandHandler : IRequestHandler<UpdateStoreCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateStoreCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateStoreCommand request, CancellationToken cancellationToken)
    {
        var store = await _unitOfWork.Repository<Store>().GetByIdAsync(request.StoreId, cancellationToken);

        if (store is null)
        {
            throw new NotFoundException(nameof(Store), request.StoreId);
        }

        if (store.OwnerId != request.RequestingUserId)
        {
            return Result.Failure("You are not authorized to update this store.");
        }

        store.UpdateDetails(request.Name, request.Description);

        _unitOfWork.Repository<Store>().Update(store);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Store updated successfully.");
    }
}