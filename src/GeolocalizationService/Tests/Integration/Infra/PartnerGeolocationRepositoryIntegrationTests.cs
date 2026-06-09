
using FluentAssertions;
using GeolocalizationService.Application.PartnerGeolocation.UseCases;
using GeolocalizationService.Application.Shared.DataTransferObjects;
using GeolocalizationService.Domain.PartnerGeolocation.Repositories;
using GeolocalizationService.Domain.Shared.ValueObjects;
using GeolocalizationService.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace GeolocalizationService.Tests.Integration.Infra;

[Collection("mongo")]
public class PartnerGeolocationRepositoryIntegrationTests
{
    private readonly MongoFixture _fixture;

    public PartnerGeolocationRepositoryIntegrationTests(MongoFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task AddAsync_ShouldPersistPartnerGeolocation()
    {
        await _fixture.ClearPartnerGeolocationsAsync();

        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IPartnerGeolocationRepository>();
        var input = GeolocationTestData.CreateValidInput();
        var entity = input.ToDomainEntity();

        await repository.AddAsync(entity);

        var retrieved = await repository.GetByPartnerIdAsync(input.Id);

        retrieved.Should().NotBeNull();
        retrieved.Id.Should().Be(input.Id);
        retrieved.Address.Cordinates.Latitude.Should().Be(input.Address.Cordinates.Latitude);
        retrieved.Address.Cordinates.Longitude.Should().Be(input.Address.Cordinates.Longitude);
    }

    [Fact]
    public async Task AddAsync_ShouldThrow_WhenPartnerAlreadyExists()
    {
        await _fixture.ClearPartnerGeolocationsAsync();

        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IPartnerGeolocationRepository>();
        var input = GeolocationTestData.CreateValidInput();
        var entity = input.ToDomainEntity();

        await repository.AddAsync(entity);

        var act = () => repository.AddAsync(entity);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdatePartnerGeolocation()
    {
        await _fixture.ClearPartnerGeolocationsAsync();

        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IPartnerGeolocationRepository>();
        var input = GeolocationTestData.CreateValidInput();
        var entity = input.ToDomainEntity();

        await repository.AddAsync(entity);

        var updatedAddress = new AddressVO(new CordinateVO(-23.56100, -46.64400));
        entity.Update(updatedAddress, entity.CoverageArea);

        await repository.UpdateAsync(entity);

        var retrieved = await repository.GetByPartnerIdAsync(input.Id);

        retrieved.Address.Cordinates.Latitude.Should().Be(-23.56100);
        retrieved.Address.Cordinates.Longitude.Should().Be(-46.64400);
    }

    [Fact]
    public async Task GetByGeolocationAsync_ShouldReturnNearestPartnerWithinCoverage()
    {
        await _fixture.ClearPartnerGeolocationsAsync();

        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IPartnerGeolocationRepository>();

        var saoPauloPartner = GeolocationTestData.CreateValidInput(Guid.NewGuid()).ToDomainEntity();
        var rioPartner = GeolocationTestData.CreateRioInput(Guid.NewGuid()).ToDomainEntity();

        await repository.AddAsync(saoPauloPartner);
        await repository.AddAsync(rioPartner);

        var results = await repository.GetByGeolocationAsync(
            new CordinateVO(GeolocationTestData.SaoPauloNearby.Latitude, GeolocationTestData.SaoPauloNearby.Longitude));

        results.Should().ContainSingle();
        results.First().Id.Should().Be(saoPauloPartner.Id);
    }

    [Fact]
    public async Task GetByGeolocationAsync_ShouldReturnEmpty_WhenLocationIsOutsideCoverageAreas()
    {
        await _fixture.ClearPartnerGeolocationsAsync();

        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IPartnerGeolocationRepository>();

        await repository.AddAsync(GeolocationTestData.CreateValidInput().ToDomainEntity());

        var results = await repository.GetByGeolocationAsync(
            new CordinateVO(GeolocationTestData.RioDeJaneiro.Latitude, GeolocationTestData.RioDeJaneiro.Longitude));

        results.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByGeolocationAsync_ShouldReturnClosestPartner_WhenMultiplePartnersCoverLocation()
    {
        await _fixture.ClearPartnerGeolocationsAsync();

        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IPartnerGeolocationRepository>();

        var closestPartner = GeolocationTestData.CreateSaoPauloInputWithAddress(
            GeolocationTestData.SaoPauloCenter,
            Guid.NewGuid()).ToDomainEntity();

        var fartherPartner = GeolocationTestData.CreateSaoPauloInputWithAddress(
            new CordinateDTO { Latitude = -23.55800, Longitude = -46.64000 },
            Guid.NewGuid()).ToDomainEntity();

        await repository.AddAsync(fartherPartner);
        await repository.AddAsync(closestPartner);

        var results = await repository.GetByGeolocationAsync(
            new CordinateVO(GeolocationTestData.SaoPauloNearby.Latitude, GeolocationTestData.SaoPauloNearby.Longitude));

        results.Should().HaveCount(2);
        results.First().Id.Should().Be(closestPartner.Id);
    }
}
