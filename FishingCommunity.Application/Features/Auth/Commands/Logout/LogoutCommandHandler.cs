using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;
using RefreshTokenEntity = FishingCommunity.Domain.Entities.Identity.RefreshToken;

namespace FishingCommunity.Application.Features.Auth.Commands.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public LogoutCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var storedTokens = await _unitOfWork.Repository<RefreshTokenEntity>()
            .FindAsync(rt => rt.UserId == request.UserId && rt.Token == request.RefreshToken, cancellationToken);

        var existingToken = storedTokens.FirstOrDefault();

        if (existingToken is null)
        {
            // Token already gone / never existed — treat logout as already-completed, not an error.
            return Result.Success("Logged out successfully.");
        }

        if (existingToken.IsActive)
        {
            existingToken.Revoke(request.IpAddress ?? "unknown");
            _unitOfWork.Repository<RefreshTokenEntity>().Update(existingToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result.Success("Logged out successfully.");
    }
}