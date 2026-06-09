
using FluentValidation;
using GeolocalizationService.Application.Shared.Validators;

namespace GeolocalizationService.Application.PartnerGeolocation.UseCases;

public sealed class GetNearestPartnerGeolocationValidator : AbstractValidator<GetNearestPartnerGeolocationInput>
{
    public GetNearestPartnerGeolocationValidator()
    {
        RuleFor(x => x.Cordinates)
            .NotNull()
            .SetValidator(new CordinateDTOValidator());
    }
}
