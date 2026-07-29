using FishingCommunity.Domain.Entities.Shop;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FishingCommunity.Application.Features.Shop.ShippingAddresses.Queries.GetMyAddresses;

public class GetMyAddressesQueryHandler : IRequestHandler<GetMyAddressesQuery, Result<List<ShippingAddressDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetMyAddressesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<ShippingAddressDto>>> Handle(GetMyAddressesQuery request, CancellationToken cancellationToken)
    {
        var addresses = await _unitOfWork.Repository<ShippingAddress>().Query()
            .Where(a => a.UserId == request.UserId)
            .OrderByDescending(a => a.IsDefault)
            .Select(a => new ShippingAddressDto
            {
                Id = a.Id,
                FullName = a.FullName,
                PhoneNumber = a.PhoneNumber,
                AddressLine1 = a.AddressLine1,
                AddressLine2 = a.AddressLine2,
                City = a.City,
                State = a.State,
                Country = a.Country,
                PostalCode = a.PostalCode,
                IsDefault = a.IsDefault
            })
            .ToListAsync(cancellationToken);

        return Result<List<ShippingAddressDto>>.Success(addresses);
    }
}