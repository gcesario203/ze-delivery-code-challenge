

using FluentValidation;
using GeolocalizationService.Application.PartnerGeolocation.DataTransferObjects;
using GeolocalizationService.Application.Shared.Exceptions;
using GeolocalizationService.Application.Shared.ValueObjects;
using GeolocalizationService.Domain.PartnerGeolocation.Repositories;
using Microsoft.Extensions.Logging;

namespace GeolocalizationService.Application.PartnerGeolocation.UseCases;

public sealed class CreatePartnerGeolocation : ICreatePartnerGeolocation
{
    private readonly IPartnerGeolocationRepository _repository;
    private readonly IValidator<CreatePartnerGeolocationInput> _validator;
    private readonly ILogger<CreatePartnerGeolocation> _logger;

    public CreatePartnerGeolocation(
        IPartnerGeolocationRepository repository,
        IValidator<CreatePartnerGeolocationInput> validator,
        ILogger<CreatePartnerGeolocation> logger)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<PartnerGeolocationViewModel> ExecuteAsync(CreatePartnerGeolocationInput input)
    {
        _logger.LogInformation("Starting CreatePartnerGeolocation use case execution.");

        var validationResult = _validator.Validate(input);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for CreatePartnerGeolocation input: {Errors}", validationResult.Errors);

            throw new UseCaseValidationException(
                "Validation failed for CreatePartnerGeolocation input",
                validationResult.Errors.Select(e => new ValidationFailureVO(e.PropertyName, e.ErrorMessage)));
        }

        var existing = await _repository.GetByPartnerIdAsync(input.Id);
        if (existing is not null)
        {
            throw new UseCaseValidationException(
                "Partner geolocation already exists.",
                [new ValidationFailureVO(nameof(CreatePartnerGeolocationInput.Id), "Partner geolocation with the given id already exists.")]);
        }

        var partnerGeolocation = input.ToDomainEntity();

        await _repository.AddAsync(partnerGeolocation);

        _logger.LogInformation("CreatePartnerGeolocation use case execution completed successfully.");

        return partnerGeolocation.ToViewModel();
    }
}
