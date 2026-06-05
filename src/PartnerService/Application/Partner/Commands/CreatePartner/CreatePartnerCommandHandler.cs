using Microsoft.Extensions.Logging;
using PartnerService.Application.Shared.Contracts;
using PartnerService.Application.Shared.Exceptions;
using PartnerService.Application.Shared.ValueObjects;
using PartnerService.Domain.Partner.Repositories;

namespace PartnerService.Application.Partner.Commands.CreatePartner;

public sealed class CreatePartnerCommandHandler
{
    private readonly IPartnerCommandRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    private readonly CreatePartnerCommandValidator _validator;

    private readonly ILogger<CreatePartnerCommandHandler> _logger;

    private readonly IPartnerQueryRepository _partnerQueryRepository;

    public CreatePartnerCommandHandler(IPartnerCommandRepository repository,
                                      IUnitOfWork unitOfWork,
                                      CreatePartnerCommandValidator validator,
                                        IPartnerQueryRepository partnerQueryRepository,
                                      ILogger<CreatePartnerCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
        _partnerQueryRepository = partnerQueryRepository;
    }

    public async Task Handle(CreatePartnerCommand command)
    {
        _logger.LogInformation("Handling CreatePartnerCommand for document: {Document}", command.Document);

        var validationResult = _validator.Validate(command);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for CreatePartnerCommand: {Errors}", validationResult.Errors);
            throw new CommandValidationException("Validation failed for CreatePartnerCommand",
                                                validationResult.Errors.Select(e => new ValidationFailure(e.PropertyName, e.ErrorMessage)));
        }

        var existingPartner = await _partnerQueryRepository.GetByCnpjAsync(command.Document);

        if (existingPartner != null)
        {
            _logger.LogWarning("Partner with document {Document} already exists.", command.Document);
            throw new CommandValidationException("Partner with the given document already exists.",
                                                new List<ValidationFailure> { new ValidationFailure("Document", "Partner with the given document already exists.") });
        }

        await _unitOfWork.BeginTransactionAsync();

        var partner = command.ToDomainEntity();

        await _repository.AddAsync(partner);

        await _unitOfWork.CommitAsync();

        await _unitOfWork.CommitTransactionAsync();

        _logger.LogInformation("Successfully created partner with document: {Document}", command.Document);
    }
}