using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;
using RefreshTokenEntity = FishingCommunity.Domain.Entities.Identity.RefreshToken;

namespace FishingCommunity.Application.Features.Auth.Commands.DeleteAccount;

public class DeleteAccountCommandHandler : IRequestHandler<DeleteAccountCommand, Result>
{
    private readonly IIdentityService _identityService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteAccountCommandHandler(IIdentityService identityService, IUnitOfWork unitOfWork)
    {
        _identityService = identityService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        // Re-verify the password before allowing this destructive action,
        // even though the user is already authenticated (defense in depth).
        var profile = await _identityService.GetUserProfileAsync(request.UserId, cancellationToken);

        if (profile is null)
        {
            return Result.Failure("Account not found.");
        }

        var (passwordValid, _, _) = await _identityService.ValidateCredentialsAsync(
            profile.Email, request.Password, cancellationToken);

        if (!passwordValid)
        {
            return Result.Failure("Incorrect password. Account deletion cancelled.");
        }

        var deleteResult = await _identityService.DeleteUserAsync(request.UserId, cancellationToken);

        if (!deleteResult.Succeeded)
        {
            return Result.Failure(deleteResult.Errors);
        }

        // Revoke all active refresh tokens so no existing session can continue to be used.
        var activeTokens = await _unitOfWork.Repository<RefreshTokenEntity>()
            .FindAsync(rt => rt.UserId == request.UserId && rt.IsActive, cancellationToken);

        foreach (var token in activeTokens)
        {
            token.Revoke("account-deletion");
            _unitOfWork.Repository<RefreshTokenEntity>().Update(token);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Your account has been deleted.");
    }
}