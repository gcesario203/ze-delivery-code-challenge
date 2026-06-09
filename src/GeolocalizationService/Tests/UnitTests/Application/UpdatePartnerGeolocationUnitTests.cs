
using FluentAssertions;
using GeolocalizationService.Application.PartnerGeolocation.UseCases;
using GeolocalizationService.Application.Shared.Exceptions;
using GeolocalizationService.Domain.PartnerGeolocation.Entities;
using GeolocalizationService.Domain.PartnerGeolocation.Repositories;
using GeolocalizationService.Tests.Fixtures;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace GeolocalizationService.Tests.UnitTests.Application;

public class UpdatePartnerGeolocationUnitTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldUpdatePartnerGeolocation_WhenPartnerExists()
    {
        var partnerId = Guid.NewGuid();
        var repository = Substitute.For<IPartnerGeolocationRepository>();
        var validator = new UpdatePartnerGeolocationValidator();
        var logger = Substitute.For<ILogger<UpdatePartnerGeolocation>>();
        var useCase = new UpdatePartnerGeolocation(repository, validator, logger);
        var existing = GeolocationTestData.CreateValidInput(partnerId).ToDomainEntity();
        var updateInput = GeolocationTestData.CreateValidUpdateInput(partnerId);

        repository.GetByPartnerIdAsync(partnerId).Returns(existing);

        var result = await useCase.ExecuteAsync(updateInput);

        result.Should().NotBeNull();
        result.Address.Cordinates.Latitude.Should().Be(updateInput.Address.Cordinates.Latitude);
        await repository.Received(1).UpdateAsync(Arg.Any<PartnerGeolocationEntity>());
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenPartnerDoesNotExist()
    {
        var repository = Substitute.For<IPartnerGeolocationRepository>();
        var validator = new UpdatePartnerGeolocationValidator();
        var logger = Substitute.For<ILogger<UpdatePartnerGeolocation>>();
        var useCase = new UpdatePartnerGeolocation(repository, validator, logger);
        var partnerId = Guid.NewGuid();

        repository.GetByPartnerIdAsync(partnerId).Returns((PartnerGeolocationEntity)null);

        var act = () => useCase.ExecuteAsync(GeolocationTestData.CreateValidUpdateInput(partnerId));

        await act.Should().ThrowAsync<UseCaseValidationException>();
    }
}
