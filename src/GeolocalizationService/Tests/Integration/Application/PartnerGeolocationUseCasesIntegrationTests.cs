
using FluentAssertions;
using GeolocalizationService.Application.PartnerGeolocation.UseCases;
using GeolocalizationService.Application.Shared.DataTransferObjects;
using GeolocalizationService.Application.Shared.Exceptions;
using GeolocalizationService.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace GeolocalizationService.Tests.Integration.Application;

[Collection("mongo")]
public class PartnerGeolocationUseCasesIntegrationTests
{
    private readonly MongoFixture _fixture;

    public PartnerGeolocationUseCasesIntegrationTests(MongoFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CreateAndGet_ShouldPersistAndRetrievePartnerGeolocation()
    {
        await _fixture.ClearPartnerGeolocationsAsync();

        using var scope = _fixture.ServiceProvider.CreateScope();
        var create = scope.ServiceProvider.GetRequiredService<ICreatePartnerGeolocation>();
        var get = scope.ServiceProvider.GetRequiredService<IGetPartnerGeolocationByPartnerId>();
        var input = GeolocationTestData.CreateValidInput();

        var created = await create.ExecuteAsync(input);

        created.Should().NotBeNull();

        var retrieved = await get.ExecuteAsync(new GetPartnerGeolocationByPartnerIdInput
        {
            PartnerId = input.Id
        });

        retrieved.Should().NotBeNull();
        retrieved.Id.Should().Be(input.Id);
    }

    [Fact]
    public async Task Update_ShouldUpdatePartnerGeolocation()
    {
        await _fixture.ClearPartnerGeolocationsAsync();

        using var scope = _fixture.ServiceProvider.CreateScope();
        var create = scope.ServiceProvider.GetRequiredService<ICreatePartnerGeolocation>();
        var update = scope.ServiceProvider.GetRequiredService<IUpdatePartnerGeolocation>();
        var get = scope.ServiceProvider.GetRequiredService<IGetPartnerGeolocationByPartnerId>();
        var input = GeolocationTestData.CreateValidInput();

        await create.ExecuteAsync(input);

        var updated = await update.ExecuteAsync(GeolocationTestData.CreateValidUpdateInput(input.Id));

        updated.Address.Cordinates.Latitude.Should().Be(-23.56100);

        var retrieved = await get.ExecuteAsync(new GetPartnerGeolocationByPartnerIdInput { PartnerId = input.Id });
        retrieved.Address.Cordinates.Latitude.Should().Be(-23.56100);
    }

    [Fact]
    public async Task GetNearest_ShouldReturnClosestPartner()
    {
        await _fixture.ClearPartnerGeolocationsAsync();

        using var scope = _fixture.ServiceProvider.CreateScope();
        var create = scope.ServiceProvider.GetRequiredService<ICreatePartnerGeolocation>();
        var getNearest = scope.ServiceProvider.GetRequiredService<IGetNearestPartnerGeolocation>();

        var saoPauloInput = GeolocationTestData.CreateValidInput(Guid.NewGuid());
        await create.ExecuteAsync(saoPauloInput);

        await create.ExecuteAsync(GeolocationTestData.CreateRioInput(Guid.NewGuid()));

        var nearest = await getNearest.ExecuteAsync(new GetNearestPartnerGeolocationInput
        {
            Cordinates = GeolocationTestData.SaoPauloNearby
        });

        nearest.Should().NotBeNull();
        nearest.Id.Should().Be(saoPauloInput.Id);
    }

    [Fact]
    public async Task GetNearest_ShouldReturnNull_WhenLocationIsOutsideCoverage()
    {
        await _fixture.ClearPartnerGeolocationsAsync();

        using var scope = _fixture.ServiceProvider.CreateScope();
        var create = scope.ServiceProvider.GetRequiredService<ICreatePartnerGeolocation>();
        var getNearest = scope.ServiceProvider.GetRequiredService<IGetNearestPartnerGeolocation>();

        await create.ExecuteAsync(GeolocationTestData.CreateValidInput());

        var nearest = await getNearest.ExecuteAsync(new GetNearestPartnerGeolocationInput
        {
            Cordinates = GeolocationTestData.RioDeJaneiro
        });

        nearest.Should().BeNull();
    }

    [Fact]
    public async Task Create_ShouldThrow_WhenDuplicatePartner()
    {
        await _fixture.ClearPartnerGeolocationsAsync();

        using var scope = _fixture.ServiceProvider.CreateScope();
        var create = scope.ServiceProvider.GetRequiredService<ICreatePartnerGeolocation>();
        var input = GeolocationTestData.CreateValidInput();

        await create.ExecuteAsync(input);

        var act = () => create.ExecuteAsync(input);

        await act.Should().ThrowAsync<UseCaseValidationException>();
    }
}
