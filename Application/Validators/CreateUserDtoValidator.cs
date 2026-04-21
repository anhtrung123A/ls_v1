using app.Application.DTOs;
using app.Domain.Constants;
using FluentValidation;

namespace app.Application.Validators;

public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserDtoValidator()
    {
        RuleFor(x => x.Fullname)
            .NotEmpty()
            .MaximumLength(UserConstants.FullnameMaxLength);

        RuleFor(x => x.Firstname)
            .MaximumLength(UserConstants.FirstnameMaxLength);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(UserConstants.EmailMaxLength);

        RuleFor(x => x.Phonenumber)
            .MaximumLength(UserConstants.PhoneNumberMaxLength);
    }
}
