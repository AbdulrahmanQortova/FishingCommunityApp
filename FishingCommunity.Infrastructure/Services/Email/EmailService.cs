using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Application.Common.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace FishingCommunity.Infrastructure.Services.Email;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
    {
        _emailSettings = emailSettings.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;

        message.Body = new BodyBuilder { HtmlBody = body }.ToMessageBody();

        try
        {
            using var client = new SmtpClient();

            await client.ConnectAsync(
                _emailSettings.SmtpHost,
                _emailSettings.SmtpPort,
                _emailSettings.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None,
                cancellationToken);

            await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password, cancellationToken);
            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);
        }
        catch (Exception ex)
        {
            // Email failures shouldn't crash the calling flow (e.g. registration succeeding
            // but the confirmation email failing to send); log and swallow instead.
            _logger.LogError(ex, "Failed to send email to {Recipient} with subject {Subject}", to, subject);
        }
    }

    public async Task SendEmailVerificationAsync(string to, string firstName, string verificationCode, CancellationToken cancellationToken = default)
    {
        var subject = "Verify your Fishing Community account";
        var body = $"""
            <div style="font-family: Arial, sans-serif; max-width: 600px; margin: auto;">
                <h2>Hi {firstName},</h2>
                <p>Thanks for joining Fishing Community! Please use the code below to verify your email address:</p>
                <div style="font-size: 28px; font-weight: bold; letter-spacing: 4px; background: #f2f2f2; padding: 16px; text-align: center; border-radius: 8px;">
                    {verificationCode}
                </div>
                <p>This code expires in 30 minutes. If you didn't create this account, you can safely ignore this email.</p>
            </div>
            """;

        await SendEmailAsync(to, subject, body, cancellationToken);
    }

    public async Task SendPasswordResetAsync(string to, string firstName, string resetToken, CancellationToken cancellationToken = default)
    {
        var subject = "Reset your Fishing Community password";
        var body = $"""
            <div style="font-family: Arial, sans-serif; max-width: 600px; margin: auto;">
                <h2>Hi {firstName},</h2>
                <p>We received a request to reset your password. Use the token below in the app to set a new password:</p>
                <div style="font-size: 18px; font-weight: bold; word-break: break-all; background: #f2f2f2; padding: 16px; text-align: center; border-radius: 8px;">
                    {resetToken}
                </div>
                <p>This token expires in 30 minutes. If you didn't request this, you can safely ignore this email.</p>
            </div>
            """;

        await SendEmailAsync(to, subject, body, cancellationToken);
    }
}