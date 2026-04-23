using app.Application.DTOs;
using app.Application.Errors;
using app.Domain.Constants;
using FluentValidation;

namespace app.Application.Validators;

public class CreateBranchUserDtoValidator : AbstractValidator<CreateBranchUserDto>
{
    public CreateBranchUserDtoValidator()
    {
        RuleFor(x => x.Lastname)
            .NotEmpty()
            .MaximumLength(UserConstants.LastnameMaxLength);

        RuleFor(x => x.Firstname)
            .MaximumLength(UserConstants.FirstnameMaxLength);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(UserConstants.EmailMaxLength);

        RuleFor(x => x.Phonenumber)
            .MaximumLength(UserConstants.PhoneNumberMaxLength);

        RuleFor(x => x.BranchId).Must(x => x > 0);
        RuleFor(x => x.RoleId).Must(x => x > 0);
    }
}
