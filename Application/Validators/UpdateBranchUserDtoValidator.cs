using app.Application.DTOs;
using app.Application.Errors;
using app.Domain.Constants;
using FluentValidation;

namespace app.Application.Validators;

public class UpdateBranchUserDtoValidator : AbstractValidator<UpdateBranchUserDto>
{
    public UpdateBranchUserDtoValidator()
    {
        RuleFor(x => x.Status)
            .Must(status => status is BranchUserStatusConstants.Active or BranchUserStatusConstants.Blocked)
            .WithMessage(AppErrors.BranchUser.InvalidStatus);
    }
}
