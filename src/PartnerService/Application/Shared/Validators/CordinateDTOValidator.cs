

namespace PartnerService.Application.Shared.Validators;

using PartnerService.Application.Shared.DataTransferObjects;
using FluentValidation;

public class CordinateDTOValidator : AbstractValidator<CordinateDTO>
{
    public CordinateDTOValidator()
    {
        RuleFor(c => c.Latitude)
            .InclusiveBetween(-90, 90)
            .WithMessage("Latitude must be between -90 and 90.");

        RuleFor(c => c.Longitude)
            .InclusiveBetween(-180, 180)
            .WithMessage("Longitude must be between -180 and 180.");
    }
}

