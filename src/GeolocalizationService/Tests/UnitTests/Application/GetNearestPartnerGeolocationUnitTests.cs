
using FluentAssertions;
using GeolocalizationService.Application.PartnerGeolocation.UseCases;
using GeolocalizationService.Application.Shared.Exceptions;
using GeolocalizationService.Domain.PartnerGeolocation.Entities;
using GeolocalizationService.Domain.PartnerGeolocation.Repositories;
using GeolocalizationService.Domain.Shared.ValueObjects;
using GeolocalizationService.Application.Shared.DataTransferObjects;
using GeolocalizationService.Tests.Fixtures;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace GeolocalizationService.Tests.UnitTests.Application;

public class GetNearestPartnerGeolocationUnitTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldReturnNearestPartner_WhenPartnersExist()
    {
        var repository = Substitute.For<IPartnerGeolocationRepository>();
        var validator = new GetNearestPartnerGeolocationValidator();
        var logger = Substitute.For<ILogger<GetNearestPartnerGeolocation>>();
        var useCase = new GetNearestPartnerGeolocation(repository, validator, logger);
        var nearest = GeolocationTestData.CreateValidInput().ToDomainEntity();

        repository.GetByGeolocationAsync(Arg.Any<CordinateVO>())
            .Returns(new[] { nearest });

        var result = await useCase.ExecuteAsync(new GetNearestPartnerGeolocationInput
        {
            Cordinates = GeolocationTestData.SaoPauloNearby
        });

        result.Should().NotBeNull();
        result.Id.Should().Be(nearest.Id);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnNull_WhenNoPartnersExist()
    {
        var repository = Substitute.For<IPartnerGeolocationRepository>();
        var validator = new GetNearestPartnerGeolocationValidator();
        var logger = Substitute.For<ILogger<GetNearestPartnerGeolocation>>();
        var useCase = new GetNearestPartnerGeolocation(repository, validator, logger);

        repository.GetByGeolocationAsync(Arg.Any<CordinateVO>())
            .Returns(Enumerable.Empty<PartnerGeolocationEntity>());

        var result = await useCase.ExecuteAsync(new GetNearestPartnerGeolocationInput
        {
            Cordinates = GeolocationTestData.SaoPauloCenter
        });

        result.Should().BeNull();
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenCoordinatesAreInvalid()
    {
        var repository = Substitute.For<IPartnerGeolocationRepository>();
        var validator = new GetNearestPartnerGeolocationValidator();
        var logger = Substitute.For<ILogger<GetNearestPartnerGeolocation>>();
        var useCase = new GetNearestPartnerGeolocation(repository, validator, logger);

        var act = () => useCase.ExecuteAsync(new GetNearestPartnerGeolocationInput
        {
            Cordinates = new CordinateDTO
            {
                Latitude = 999,
                Longitude = 999
            }
        });

        await act.Should().ThrowAsync<UseCaseValidationException>();
    }
}
