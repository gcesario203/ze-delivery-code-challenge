

using FluentValidation;
using GeolocalizationService.Application.PartnerGeolocation.DataTransferObjects;
using GeolocalizationService.Application.Shared.Exceptions;
using GeolocalizationService.Application.Shared.ValueObjects;
using GeolocalizationService.Domain.PartnerGeolocation.Entities;
using GeolocalizationService.Domain.PartnerGeolocation.Repositories;
using Microsoft.Extensions.Logging;

namespace GeolocalizationService.Application.PartnerGeolocation.UseCases;

public sealed class UpdatePartnerGeolocation : IUpdatePartnerGeolocation
{
    private readonly IPartnerGeolocationRepository _repository;
    private readonly IValidator<UpdatePartnerGeolocationInput> _validator;
    private readonly ILogger<UpdatePartnerGeolocation> _logger;

    public UpdatePartnerGeolocation(
        IPartnerGeolocationRepository repository,
        IValidator<UpdatePartnerGeolocationInput> validator,
        ILogger<UpdatePartnerGeolocation> logger)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<PartnerGeolocationViewModel> ExecuteAsync(UpdatePartnerGeolocationInput input)
    {
        _logger.LogInformation("Starting UpdatePartnerGeolocation use case for partner {PartnerId}.", input.Id);

        var validationResult = _validator.Validate(input);

        if (!validationResult.IsValid)
        {
            throw new UseCaseValidationException(
                "Validation failed for UpdatePartnerGeolocation input",
                validationResult.Errors.Select(e => new ValidationFailureVO(e.PropertyName, e.ErrorMessage)));
        }

        var existing = await _repository.GetByPartnerIdAsync(input.Id);

        if (existing is null)
        {
            throw new UseCaseValidationException(
                "Partner geolocation not found.",
                [new ValidationFailureVO(nameof(UpdatePartnerGeolocationInput.Id), "Partner geolocation with the given id was not found.")]);
        }

        existing.Update(input.Address.ToVO(), input.CoverageArea.ToVO());

        await _repository.UpdateAsync(existing);

        _logger.LogInformation("UpdatePartnerGeolocation use case completed successfully.");

        return existing.ToViewModel();
    }
}
