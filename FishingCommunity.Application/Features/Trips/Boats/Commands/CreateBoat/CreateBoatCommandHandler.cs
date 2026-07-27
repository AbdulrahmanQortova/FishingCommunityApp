using FishingCommunity.Domain.Entities.Trips;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Trips.Boats.Commands.CreateBoat;

public class CreateBoatCommandHandler : IRequestHandler<CreateBoatCommand, Result<CreateBoatResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateBoatCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CreateBoatResponse>> Handle(CreateBoatCommand request, CancellationToken cancellationToken)
    {
        var registrationExists = await _unitOfWork.Repository<Boat>()
            .AnyAsync(b => b.RegistrationNumber == request.RegistrationNumber, cancellationToken);

        if (registrationExists)
        {
            return Result<CreateBoatResponse>.Failure("A boat with this registration number already exists.");
        }

        var boat = new Boat(request.OwnerId, request.Name, request.RegistrationNumber, request.Capacity, request.Description);

        await _unitOfWork.Repository<Boat>().AddAsync(boat, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new CreateBoatResponse
        {
            BoatId = boat.Id,
            Name = boat.Name,
            RegistrationNumber = boat.RegistrationNumber,
            Capacity = boat.Capacity
        };

        return Result<CreateBoatResponse>.Success(response, "Boat created successfully.");
    }
}