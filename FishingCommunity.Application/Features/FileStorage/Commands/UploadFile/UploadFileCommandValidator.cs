using FluentValidation;

namespace FishingCommunity.Application.Features.FileStorage.Commands.UploadFile;

public class UploadFileCommandValidator : AbstractValidator<UploadFileCommand>
{
    private static readonly string[] AllowedImageTypes = { "image/jpeg", "image/png", "image/webp" };
    private static readonly string[] AllowedAudioTypes = { "audio/mpeg", "audio/mp4", "audio/webm", "audio/ogg" };
    private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB

    public UploadFileCommandValidator()
    {
        RuleFor(x => x.FileName).NotEmpty();
        RuleFor(x => x.Folder).NotEmpty();

        RuleFor(x => x.FileSizeBytes)
            .GreaterThan(0).WithMessage("File is empty.")
            .LessThanOrEqualTo(MaxFileSizeBytes).WithMessage("File size cannot exceed 10 MB.");

        RuleFor(x => x.ContentType)
            .Must(ct => AllowedImageTypes.Contains(ct) || AllowedAudioTypes.Contains(ct))
            .WithMessage("Only JPEG, PNG, WEBP images or MP3/M4A/WEBM/OGG audio files are allowed.");
    }
}