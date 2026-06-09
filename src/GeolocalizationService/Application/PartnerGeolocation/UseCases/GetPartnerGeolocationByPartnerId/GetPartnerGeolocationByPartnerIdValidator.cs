
using FluentValidation;

namespace GeolocalizationService.Application.PartnerGeolocation.UseCases;

public sealed class GetPartnerGeolocationByPartnerIdValidator : AbstractValidator<GetPartnerGeolocationByPartnerIdInput>
{
    public GetPartnerGeolocationByPartnerIdValidator()
    {
        RuleFor(x => x.PartnerId)
            .NotEmpty()
            .WithMessage("Partner id is required.");
    }
}
