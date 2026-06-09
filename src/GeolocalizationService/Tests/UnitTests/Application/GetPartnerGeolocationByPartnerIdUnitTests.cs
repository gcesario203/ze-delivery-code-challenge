
using FluentAssertions;
using GeolocalizationService.Application.PartnerGeolocation.UseCases;
using GeolocalizationService.Domain.PartnerGeolocation.Entities;
using GeolocalizationService.Application.Shared.Exceptions;
using GeolocalizationService.Domain.PartnerGeolocation.Repositories;
using GeolocalizationService.Tests.Fixtures;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace GeolocalizationService.Tests.UnitTests.Application;

public class GetPartnerGeolocationByPartnerIdUnitTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldReturnViewModel_WhenPartnerExists()
    {
        var partnerId = Guid.NewGuid();
        var repository = Substitute.For<IPartnerGeolocationRepository>();
        var validator = new GetPartnerGeolocationByPartnerIdValidator();
        var logger = Substitute.For<ILogger<GetPartnerGeolocationByPartnerId>>();
        var useCase = new GetPartnerGeolocationByPartnerId(repository, validator, logger);
        var input = GeolocationTestData.CreateValidInput(partnerId);

        repository.GetByPartnerIdAsync(partnerId).Returns(input.ToDomainEntity());

        var result = await useCase.ExecuteAsync(new GetPartnerGeolocationByPartnerIdInput { PartnerId = partnerId });

        result.Should().NotBeNull();
        result.Id.Should().Be(partnerId);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnNull_WhenPartnerDoesNotExist()
    {
        var repository = Substitute.For<IPartnerGeolocationRepository>();
        var validator = new GetPartnerGeolocationByPartnerIdValidator();
        var logger = Substitute.For<ILogger<GetPartnerGeolocationByPartnerId>>();
        var useCase = new GetPartnerGeolocationByPartnerId(repository, validator, logger);

        repository.GetByPartnerIdAsync(Arg.Any<Guid>()).Returns((PartnerGeolocationEntity)null);

        var result = await useCase.ExecuteAsync(new GetPartnerGeolocationByPartnerIdInput { PartnerId = Guid.NewGuid() });

        result.Should().BeNull();
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenPartnerIdIsEmpty()
    {
        var repository = Substitute.For<IPartnerGeolocationRepository>();
        var validator = new GetPartnerGeolocationByPartnerIdValidator();
        var logger = Substitute.For<ILogger<GetPartnerGeolocationByPartnerId>>();
        var useCase = new GetPartnerGeolocationByPartnerId(repository, validator, logger);

        var act = () => useCase.ExecuteAsync(new GetPartnerGeolocationByPartnerIdInput { PartnerId = Guid.Empty });

        await act.Should().ThrowAsync<UseCaseValidationException>();
    }
}
