

using FluentValidation;
using GeolocalizationService.Application.PartnerGeolocation.DataTransferObjects;
using GeolocalizationService.Application.Shared.Exceptions;
using GeolocalizationService.Application.Shared.ValueObjects;
using GeolocalizationService.Domain.PartnerGeolocation.Repositories;
using Microsoft.Extensions.Logging;

namespace GeolocalizationService.Application.PartnerGeolocation.UseCases;

public sealed class GetPartnerGeolocationByPartnerId : IGetPartnerGeolocationByPartnerId
{
    private readonly IPartnerGeolocationRepository _repository;
    private readonly IValidator<GetPartnerGeolocationByPartnerIdInput> _validator;
    private readonly ILogger<GetPartnerGeolocationByPartnerId> _logger;

    public GetPartnerGeolocationByPartnerId(
        IPartnerGeolocationRepository repository,
        IValidator<GetPartnerGeolocationByPartnerIdInput> validator,
        ILogger<GetPartnerGeolocationByPartnerId> logger)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<PartnerGeolocationViewModel> ExecuteAsync(GetPartnerGeolocationByPartnerIdInput input)
    {
        _logger.LogInformation("Starting GetPartnerGeolocationByPartnerId use case for partner {PartnerId}.", input.PartnerId);

        var validationResult = _validator.Validate(input);

        if (!validationResult.IsValid)
        {
            throw new UseCaseValidationException(
                "Validation failed for GetPartnerGeolocationByPartnerId input",
                validationResult.Errors.Select(e => new ValidationFailureVO(e.PropertyName, e.ErrorMessage)));
        }

        var partnerGeolocation = await _repository.GetByPartnerIdAsync(input.PartnerId);

        if (partnerGeolocation is null)
            return null;

        return partnerGeolocation.ToViewModel();
    }
}
