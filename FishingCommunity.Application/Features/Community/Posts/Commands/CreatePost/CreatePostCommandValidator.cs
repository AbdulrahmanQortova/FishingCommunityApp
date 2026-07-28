using FluentValidation;

namespace FishingCommunity.Application.Features.Community.Posts.Commands.CreatePost;

public class CreatePostCommandValidator : AbstractValidator<CreatePostCommand>
{
    public CreatePostCommandValidator()
    {
        RuleFor(x => x.AuthorId).NotEmpty();

        RuleFor(x => x.Content)
            .MaximumLength(3000).WithMessage("Post content must not exceed 3000 characters.");

        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.Content) || (x.PhotoUrls is not null && x.PhotoUrls.Count > 0))
            .WithMessage("A post must have content or at least one photo.")
            .OverridePropertyName("Content");

        RuleForEach(x => x.PhotoUrls)
            .Must(url => Uri.IsWellFormedUriString(url, UriKind.Absolute))
            .WithMessage("Each photo URL must be a valid, absolute URL.")
            .When(x => x.PhotoUrls is not null);

        RuleFor(x => x.PhotoUrls)
            .Must(urls => urls == null || urls.Count <= 10)
            .WithMessage("A post cannot have more than 10 photos.");
    }
}