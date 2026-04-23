using app.Application.DTOs;
using app.Domain.Constants;
using FluentValidation;

namespace app.Application.Validators;

public class UpdateBranchDtoValidator : AbstractValidator<UpdateBranchDto>
{
    public UpdateBranchDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(BranchConstants.NameMaxLength);

        RuleFor(x => x.Description)
            .MaximumLength(BranchConstants.DescriptionMaxLength);

        RuleFor(x => x.AddressLine1)
            .MaximumLength(BranchConstants.AddressLineMaxLength);

        RuleFor(x => x.AddressLine2)
            .MaximumLength(BranchConstants.AddressLineMaxLength);

        RuleFor(x => x.Ward)
            .MaximumLength(BranchConstants.WardMaxLength);

        RuleFor(x => x.District)
            .MaximumLength(BranchConstants.DistrictMaxLength);

        RuleFor(x => x.City)
            .MaximumLength(BranchConstants.CityMaxLength);

        RuleFor(x => x.PostalCode)
            .MaximumLength(BranchConstants.PostalCodeMaxLength);

        RuleFor(x => x.Country)
            .MaximumLength(BranchConstants.CountryMaxLength);
    }
}
