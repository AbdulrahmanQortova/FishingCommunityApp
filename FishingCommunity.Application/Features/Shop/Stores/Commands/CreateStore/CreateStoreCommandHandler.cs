using FishingCommunity.Domain.Entities.Shop;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Shop.Stores.Commands.CreateStore;

public class CreateStoreCommandHandler : IRequestHandler<CreateStoreCommand, Result<CreateStoreResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateStoreCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CreateStoreResponse>> Handle(CreateStoreCommand request, CancellationToken cancellationToken)
    {
        var alreadyHasStore = await _unitOfWork.Repository<Store>()
            .AnyAsync(s => s.OwnerId == request.OwnerId, cancellationToken);

        if (alreadyHasStore)
        {
            return Result<CreateStoreResponse>.Failure("You already have a store registered.");
        }

        var store = new Store(request.OwnerId, request.Name, request.Description);

        await _unitOfWork.Repository<Store>().AddAsync(store, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new CreateStoreResponse
        {
            StoreId = store.Id,
            Name = store.Name
        };

        return Result<CreateStoreResponse>.Success(response, "Store created successfully. It's pending review before it goes live.");
    }
}