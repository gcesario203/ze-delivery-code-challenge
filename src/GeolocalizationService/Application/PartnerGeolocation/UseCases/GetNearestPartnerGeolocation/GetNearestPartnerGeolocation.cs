

using FluentValidation;
using GeolocalizationService.Application.PartnerGeolocation.DataTransferObjects;
using GeolocalizationService.Application.Shared.Exceptions;
using GeolocalizationService.Application.Shared.ValueObjects;
using GeolocalizationService.Domain.PartnerGeolocation.Repositories;
using Microsoft.Extensions.Logging;

namespace GeolocalizationService.Application.PartnerGeolocation.UseCases;

public sealed class GetNearestPartnerGeolocation : IGetNearestPartnerGeolocation
{
    private readonly IPartnerGeolocationRepository _repository;
    private readonly IValidator<GetNearestPartnerGeolocationInput> _validator;
    private readonly ILogger<GetNearestPartnerGeolocation> _logger;

    public GetNearestPartnerGeolocation(
        IPartnerGeolocationRepository repository,
        IValidator<GetNearestPartnerGeolocationInput> validator,
        ILogger<GetNearestPartnerGeolocation> logger)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<PartnerGeolocationViewModel> ExecuteAsync(GetNearestPartnerGeolocationInput input)
    {
        _logger.LogInformation("Starting GetNearestPartnerGeolocation use case.");

        var validationResult = _validator.Validate(input);

        if (!validationResult.IsValid)
        {
            throw new UseCaseValidationException(
                "Validation failed for GetNearestPartnerGeolocation input",
                validationResult.Errors.Select(e => new ValidationFailureVO(e.PropertyName, e.ErrorMessage)));
        }

        var coordinate = new Domain.Shared.ValueObjects.CordinateVO(
            input.Cordinates.Latitude,
            input.Cordinates.Longitude);

        var partners = await _repository.GetByGeolocationAsync(coordinate);
        var nearest = partners.FirstOrDefault();

        if (nearest is null)
            return null;

        return nearest.ToViewModel();
    }
}
