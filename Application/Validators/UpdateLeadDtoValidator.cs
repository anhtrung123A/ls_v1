using app.Application.DTOs;
using app.Domain.Constants;
using FluentValidation;

namespace app.Application.Validators;

public class UpdateLeadDtoValidator : AbstractValidator<UpdateLeadDto>
{
    public UpdateLeadDtoValidator()
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
