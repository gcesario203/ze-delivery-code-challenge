using FluentValidation;
using PartnerService.Application.Shared.Validators;
using PartnerService.Domain.Shared.ValueObjects;

namespace PartnerService.Application.Partner.Commands.CreatePartner;

public sealed class CreatePartnerCommandValidator : AbstractValidator<CreatePartnerCommand>
{
    public CreatePartnerCommandValidator()
    {
        RuleFor(x => x.TradingName)
            .NotEmpty().WithMessage("Trading name is required.")
            .MaximumLength(255).WithMessage("Trading name must not exceed 255 characters.");

        RuleFor(x => x.OwnerName)
            .NotEmpty().WithMessage("Owner name is required.")
            .MaximumLength(255).WithMessage("Owner name must not exceed 255 characters.");

        RuleFor(x => x.Document)
            .NotEmpty().WithMessage("Document is required.")
            .Must(CnpjVO.Validate).WithMessage("Document must be a valid CNPJ.");

        RuleFor(x => x.Address)
            .NotNull().WithMessage("Address is required.")
            .SetValidator(new AddressDTOValidator());

        RuleFor(x => x.CoverageArea)
            .NotNull().WithMessage("Coverage area is required.")
            .SetValidator(new CoverageAreaDTOValidator());
    }
}