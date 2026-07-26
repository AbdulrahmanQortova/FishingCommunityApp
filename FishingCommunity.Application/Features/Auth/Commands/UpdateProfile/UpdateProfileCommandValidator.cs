using FluentValidation;

namespace FishingCommunity.Application.Features.Auth.Commands.UpdateProfile;

public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User id is required.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(50).WithMessage("First name must not exceed 50 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(50).WithMessage("Last name must not exceed 50 characters.");

        RuleFor(x => x.Bio)
            .MaximumLength(500).WithMessage("Bio must not exceed 500 characters.")
            .When(x => x.Bio is not null);

        RuleFor(x => x.DateOfBirth)
            .Must(dob => dob is null || dob.Value.Date < DateTime.UtcNow.Date)
            .WithMessage("Date of birth must be in the past.")
            .Must(dob => dob is null || DateTime.UtcNow.Year - dob.Value.Year >= 13)
            .WithMessage("You must be at least 13 years old to use this platform.");

        RuleFor(x => x.ProfilePictureUrl)
            .MaximumLength(2048).WithMessage("Profile picture URL is too long.")
            .Must(url => Uri.IsWellFormedUriString(url, UriKind.Absolute))
            .WithMessage("Profile picture URL must be a valid, absolute URL.")
            .When(x => !string.IsNullOrWhiteSpace(x.ProfilePictureUrl));
    }
}