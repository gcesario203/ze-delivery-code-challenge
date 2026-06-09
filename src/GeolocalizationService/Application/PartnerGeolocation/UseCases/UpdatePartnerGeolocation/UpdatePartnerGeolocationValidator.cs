
using FluentValidation;
using GeolocalizationService.Application.Shared.Validators;

namespace GeolocalizationService.Application.PartnerGeolocation.UseCases;

public sealed class UpdatePartnerGeolocationValidator : AbstractValidator<UpdatePartnerGeolocationInput>
{
    public UpdatePartnerGeolocationValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required.");

        RuleFor(x => x.Address)
            .NotNull()
            .SetValidator(new AddressDTOValidator());

        RuleFor(x => x.CoverageArea)
            .NotNull()
            .SetValidator(new CoverageAreaDTOValidator());
    }
}
