using FishingCommunity.Domain.Entities.Shop;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FishingCommunity.Application.Features.Shop.ShippingAddresses.Commands.AddShippingAddress;

public class AddShippingAddressCommandHandler : IRequestHandler<AddShippingAddressCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddShippingAddressCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(AddShippingAddressCommand request, CancellationToken cancellationToken)
    {
        var address = new ShippingAddress(
            request.UserId, request.FullName, request.PhoneNumber, request.AddressLine1,
            request.City, request.Country, request.AddressLine2, request.State, request.PostalCode);

        if (request.SetAsDefault)
        {
            // Unset any existing default address for this user first.
            var existingDefaults = await _unitOfWork.Repository<ShippingAddress>().Query()
                .Where(a => a.UserId == request.UserId && a.IsDefault)
                .ToListAsync(cancellationToken);

            foreach (var existing in existingDefaults)
            {
                existing.UnsetDefault();
                _unitOfWork.Repository<ShippingAddress>().Update(existing);
            }

            address.SetAsDefault();
        }

        await _unitOfWork.Repository<ShippingAddress>().AddAsync(address, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(address.Id, "Shipping address added successfully.");
    }
}