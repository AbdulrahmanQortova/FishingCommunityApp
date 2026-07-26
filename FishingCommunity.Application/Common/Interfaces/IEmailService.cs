namespace FishingCommunity.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
    Task SendEmailVerificationAsync(string to, string firstName, string verificationCode, CancellationToken cancellationToken = default);
    Task SendPasswordResetAsync(string to, string firstName, string resetToken, CancellationToken cancellationToken = default);
}