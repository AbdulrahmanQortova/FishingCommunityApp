using FluentValidation;

namespace FishingCommunity.Application.Features.Chat.Commands.SendMessage;

public class SendMessageCommandValidator : AbstractValidator<SendMessageCommand>
{
    public SendMessageCommandValidator()
    {
        RuleFor(x => x.SenderId).NotEmpty();
        RuleFor(x => x.RecipientId).NotEmpty();

        RuleFor(x => x.TextContent)
            .NotEmpty().WithMessage("Message content cannot be empty.")
            .MaximumLength(2000)
            .When(x => x.Type == Domain.Enums.MessageType.Text);

        RuleFor(x => x.MediaUrl)
            .NotEmpty().WithMessage("Media URL is required for image/voice messages.")
            .Must(url => Uri.IsWellFormedUriString(url, UriKind.Absolute))
            .WithMessage("Media URL must be a valid, absolute URL.")
            .When(x => x.Type is Domain.Enums.MessageType.Image or Domain.Enums.MessageType.Voice);
    }
}