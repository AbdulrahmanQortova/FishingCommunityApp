using Asp.Versioning;
using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Application.Features.Auth.Commands.ChangePassword;
using FishingCommunity.Application.Features.Auth.Commands.DeleteAccount;
using FishingCommunity.Application.Features.Auth.Commands.ForgotPassword;
using FishingCommunity.Application.Features.Auth.Commands.Login;
using FishingCommunity.Application.Features.Auth.Commands.Logout;
using FishingCommunity.Application.Features.Auth.Commands.Register;
using FishingCommunity.Application.Features.Auth.Commands.ResendEmailVerification;
using FishingCommunity.Application.Features.Auth.Commands.ResetPassword;
using FishingCommunity.Application.Features.Auth.Commands.UpdateProfile;
using FishingCommunity.Application.Features.Auth.Commands.VerifyEmail;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FishingCommunity.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
public class AuthController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUserService;

    public AuthController(ISender sender, ICurrentUserService currentUserService)
    {
        _sender = sender;
        _currentUserService = currentUserService;
    }

    [HttpPost("register")]
    [EnableRateLimiting("AuthPolicy")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPost("login")]
    [EnableRateLimiting("AuthPolicy")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
    {
        command.IpAddress = GetClientIpAddress();

        var result = await _sender.Send(command, cancellationToken);

        return result.Succeeded ? Ok(result) : Unauthorized(result);
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken(
        [FromBody] Application.Features.Auth.Commands.RefreshToken.RefreshTokenCommand command,
        CancellationToken cancellationToken)
    {
        command.IpAddress = GetClientIpAddress();

        var result = await _sender.Send(command, cancellationToken);

        return result.Succeeded ? Ok(result) : Unauthorized(result);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] LogoutRequestDto request, CancellationToken cancellationToken)
    {
        var command = new LogoutCommand
        {
            UserId = _currentUserService.UserId!.Value,
            RefreshToken = request.RefreshToken,
            IpAddress = GetClientIpAddress()
        };

        var result = await _sender.Send(command, cancellationToken);

        return Ok(result);
    }

    [HttpPost("forgot-password")]
    [EnableRateLimiting("AuthPolicy")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPost("verify-email")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPost("resend-verification")]
    [EnableRateLimiting("AuthPolicy")]
    [AllowAnonymous]
    public async Task<IActionResult> ResendEmailVerification([FromBody] ResendEmailVerificationCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request, CancellationToken cancellationToken)
    {
        var command = new ChangePasswordCommand
        {
            UserId = _currentUserService.UserId!.Value,
            CurrentPassword = request.CurrentPassword,
            NewPassword = request.NewPassword,
            ConfirmNewPassword = request.ConfirmNewPassword
        };

        var result = await _sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPut("profile")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequestDto request, CancellationToken cancellationToken)
    {
        var command = new UpdateProfileCommand
        {
            UserId = _currentUserService.UserId!.Value,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Bio = request.Bio,
            DateOfBirth = request.DateOfBirth,
            ProfilePictureUrl = request.ProfilePictureUrl
        };

        var result = await _sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("account")]
    [Authorize]
    public async Task<IActionResult> DeleteAccount([FromBody] DeleteAccountRequestDto request, CancellationToken cancellationToken)
    {
        var command = new DeleteAccountCommand
        {
            UserId = _currentUserService.UserId!.Value,
            Password = request.Password
        };

        var result = await _sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    private string? GetClientIpAddress()
    {
        var forwardedFor = Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(forwardedFor))
        {
            return forwardedFor.Split(',').First().Trim();
        }

        return HttpContext.Connection.RemoteIpAddress?.ToString();
    }
}