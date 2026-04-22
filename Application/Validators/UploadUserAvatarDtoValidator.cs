using app.Application.DTOs;
using app.Domain.Constants;
using FluentValidation;

namespace app.Application.Validators;

public class UploadUserAvatarDtoValidator : AbstractValidator<UploadUserAvatarDto>
{
    public UploadUserAvatarDtoValidator()
    {
        RuleFor(x => x.Avatar)
            .NotNull()
            .WithMessage("Avatar file is required.");

        RuleFor(x => x.Avatar.Length)
            .GreaterThan(0)
            .WithMessage("Avatar file is empty.")
            .LessThanOrEqualTo(FileConstants.AvatarMaxSizeBytes)
            .WithMessage($"Avatar file size must be less than or equal to {FileConstants.AvatarMaxSizeBytes} bytes.")
            .When(x => x.Avatar is not null);

        RuleFor(x => x.Avatar.ContentType)
            .Must(contentType => FileConstants.AllowedAvatarContentTypes.Contains(contentType))
            .WithMessage("Avatar content type is not supported.")
            .When(x => x.Avatar is not null);
    }
}
