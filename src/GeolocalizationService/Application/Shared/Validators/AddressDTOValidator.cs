
namespace GeolocalizationService.Application.Shared.Validators;

using FluentValidation;
using GeolocalizationService.Application.Shared.DataTransferObjects;

public class AddressDTOValidator : AbstractValidator<AddressDTO>
{
    public AddressDTOValidator()
    {
        RuleFor(a => a.Cordinates)
            .NotNull()
            .SetValidator(new CordinateDTOValidator());
    }
}