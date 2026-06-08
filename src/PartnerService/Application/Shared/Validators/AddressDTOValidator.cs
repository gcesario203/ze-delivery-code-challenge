
namespace PartnerService.Application.Shared.Validators;

using PartnerService.Domain.Shared.Enums;
using FluentValidation;
using PartnerService.Application.Shared.DataTransferObjects;

public class AddressDTOValidator : AbstractValidator<AddressDTO>
{
    public AddressDTOValidator()
    {
        RuleFor(a => a.Cordinates)
            .NotNull()
            .SetValidator(new CordinateDTOValidator());
    }
}