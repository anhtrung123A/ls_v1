using app.Application.DTOs;
using app.Domain.Constants;
using FluentValidation;

namespace app.Application.Validators;

public class CreateLeadNoteDtoValidator : AbstractValidator<CreateLeadNoteDto>
{
    public CreateLeadNoteDtoValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .MaximumLength(LeadConstants.LeadNoteContentMaxLength);
    }
}
