using FluentValidation;
using Microsoft.Extensions.Logging;
using PartnerService.Application.GeoLocalization.Contracts;
using PartnerService.Application.GeoLocalization.DataTransferObjects;
using PartnerService.Application.GeoLocalization.Events;
using PartnerService.Application.Partner.Queries.GetById;
using PartnerService.Application.Shared.Contracts;
using PartnerService.Application.Shared.DataTransferObjects;
using PartnerService.Application.Shared.Exceptions;
using PartnerService.Application.Shared.Mappers;
using PartnerService.Application.Shared.ValueObjects;
using PartnerService.Domain.Partner.Repositories;
using Wolverine.Attributes;

namespace PartnerService.Application.Partner.Commands.CreatePartner;

public sealed class CreatePartnerCommandHandler
{
    private readonly IPartnerCommandRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    private readonly IValidator<CreatePartnerCommand> _validator;

    private readonly ILogger<CreatePartnerCommandHandler> _logger;

    private readonly IPartnerQueryRepository _partnerQueryRepository;


    public CreatePartnerCommandHandler(IPartnerCommandRepository repository,
                                      IUnitOfWork unitOfWork,
                                      IValidator<CreatePartnerCommand> validator,
                                        IPartnerQueryRepository partnerQueryRepository,
                                      ILogger<CreatePartnerCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
        _partnerQueryRepository = partnerQueryRepository;
    }

    [WolverineHandler]
    public async Task<PartnerViewModel> Handle(CreatePartnerCommand command)
    {
        _logger.LogInformation("Handling CreatePartnerCommand for document: {Document}", command.Document);

        var validationResult = _validator.Validate(command);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for CreatePartnerCommand: {Errors}", validationResult.Errors);
            throw new CommandValidationException("Validation failed for CreatePartnerCommand",
                                                validationResult.Errors.Select(e => new ValidationFailureVO(e.PropertyName, e.ErrorMessage)));
        }

        var existingPartner = await _partnerQueryRepository.GetByCnpjAsync(command.Document);

        if (existingPartner != null)
        {
            _logger.LogWarning("Partner with document {Document} already exists.", command.Document);
            throw new CommandValidationException("Partner with the given document already exists.",
                                                new List<ValidationFailureVO> { new ValidationFailureVO("Document", "Partner with the given document already exists.") });
        }

        await _unitOfWork.BeginTransactionAsync();

        var partner = command.ToDomainEntity(
            address: command.Address.ToVO(),
            coverageArea: command.CoverageArea.ToVO()
        );

        await _repository.AddAsync(partner);

        await _unitOfWork.CommitAsync();

        await _unitOfWork.CommitTransactionAsync();

        _logger.LogInformation("Successfully created partner with document: {Document}", command.Document);

        return partner.ToViewModel(new PartnerGeolocalizationDTO
        {
            Address = command.Address,
            CoverageArea = command.CoverageArea
        });
    }
}