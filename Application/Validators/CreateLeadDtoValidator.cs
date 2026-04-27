using app.Application.DTOs;
using app.Domain.Constants;
using FluentValidation;

namespace app.Application.Validators;

public class CreateLeadDtoValidator : AbstractValidator<CreateLeadDto>
{
    public CreateLeadDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(LeadConstants.FirstNameMaxLength);

        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(LeadConstants.FullNameMaxLength);

        RuleFor(x => x.Note)
            .MaximumLength(LeadConstants.NoteMaxLength);
    }
}
