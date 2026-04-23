using app.Application.DTOs;
using app.Domain.Constants;
using FluentValidation;

namespace app.Application.Validators;

public class UploadBranchImageDtoValidator : AbstractValidator<UploadBranchImageDto>
{
    public UploadBranchImageDtoValidator()
    {
        RuleFor(x => x.Image)
            .NotNull()
            .WithMessage("Branch image file is required.");

        RuleFor(x => x.Image.Length)
            .GreaterThan(0)
            .WithMessage("Branch image file is empty.")
            .LessThanOrEqualTo(FileConstants.AvatarMaxSizeBytes)
            .WithMessage($"Branch image file size must be less than or equal to {FileConstants.AvatarMaxSizeBytes} bytes.")
            .When(x => x.Image is not null);

        RuleFor(x => x.Image.ContentType)
            .Must(contentType => FileConstants.AllowedAvatarContentTypes.Contains(contentType))
            .WithMessage("Branch image content type is not supported.")
            .When(x => x.Image is not null);
    }
}
