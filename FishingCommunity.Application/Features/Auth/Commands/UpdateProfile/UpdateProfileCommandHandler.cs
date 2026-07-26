using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Auth.Commands.UpdateProfile;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, Result<UpdateProfileResponse>>
{
    private readonly IIdentityService _identityService;

    public UpdateProfileCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<UpdateProfileResponse>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.UpdateProfileAsync(
            request.UserId,
            request.FirstName,
            request.LastName,
            request.Bio,
            request.DateOfBirth,
            request.ProfilePictureUrl,
            cancellationToken);

        if (!result.Succeeded)
        {
            return Result<UpdateProfileResponse>.Failure(result.Errors);
        }

        var response = new UpdateProfileResponse
        {
            UserId = request.UserId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Bio = request.Bio,
            DateOfBirth = request.DateOfBirth,
            ProfilePictureUrl = request.ProfilePictureUrl
        };

        return Result<UpdateProfileResponse>.Success(response, "Profile updated successfully.");
    }
}