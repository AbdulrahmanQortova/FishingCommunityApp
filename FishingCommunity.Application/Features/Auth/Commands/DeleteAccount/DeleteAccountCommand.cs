using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Auth.Commands.DeleteAccount;

public class DeleteAccountCommand : IRequest<Result>
{
    public Guid UserId { get; set; }
    public string Password { get; set; } = string.Empty; // Re-confirmation required for this destructive action
}