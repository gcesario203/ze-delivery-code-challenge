
using FluentAssertions;
using GeolocalizationService.Application.PartnerGeolocation.UseCases;
using GeolocalizationService.Application.Shared.Exceptions;
using GeolocalizationService.Domain.PartnerGeolocation.Entities;
using GeolocalizationService.Domain.PartnerGeolocation.Repositories;
using GeolocalizationService.Tests.Fixtures;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace GeolocalizationService.Tests.UnitTests.Application;

public class CreatePartnerGeolocationUnitTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldCreatePartnerGeolocation_WhenInputIsValid()
    {
        var repository = Substitute.For<IPartnerGeolocationRepository>();
        var validator = new CreatePartnerGeolocationValidator();
        var logger = Substitute.For<ILogger<CreatePartnerGeolocation>>();
        var useCase = new CreatePartnerGeolocation(repository, validator, logger);
        var input = GeolocationTestData.CreateValidInput();

        repository.GetByPartnerIdAsync(input.Id).Returns((PartnerGeolocationEntity)null);

        var result = await useCase.ExecuteAsync(input);

        result.Should().NotBeNull();
        result.Id.Should().Be(input.Id);
        await repository.Received(1).AddAsync(Arg.Any<PartnerGeolocationEntity>());
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenInputIsInvalid()
    {
        var repository = Substitute.For<IPartnerGeolocationRepository>();
        var validator = new CreatePartnerGeolocationValidator();
        var logger = Substitute.For<ILogger<CreatePartnerGeolocation>>();
        var useCase = new CreatePartnerGeolocation(repository, validator, logger);

        var act = () => useCase.ExecuteAsync(new CreatePartnerGeolocationInput());

        await act.Should().ThrowAsync<UseCaseValidationException>();
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenPartnerAlreadyExists()
    {
        var repository = Substitute.For<IPartnerGeolocationRepository>();
        var validator = new CreatePartnerGeolocationValidator();
        var logger = Substitute.For<ILogger<CreatePartnerGeolocation>>();
        var useCase = new CreatePartnerGeolocation(repository, validator, logger);
        var input = GeolocationTestData.CreateValidInput();
        var existing = input.ToDomainEntity();

        repository.GetByPartnerIdAsync(input.Id).Returns(existing);

        var act = () => useCase.ExecuteAsync(input);

        await act.Should().ThrowAsync<UseCaseValidationException>();
    }
}
