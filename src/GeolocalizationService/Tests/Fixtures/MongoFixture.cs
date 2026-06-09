
using GeolocalizationService.Application.Shared.Configuration;
using GeolocalizationService.Infra.PartnerGeolocation.Models;
using GeolocalizationService.Infra.Shared.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mongo2Go;
using MongoDB.Driver;

namespace GeolocalizationService.Tests.Fixtures;

public class MongoFixture : IAsyncLifetime
{
    private MongoDbRunner _runner = null!;

    public IServiceProvider ServiceProvider { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        _runner = MongoDbRunner.Start();

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["ConnectionStrings:MongoDB"] = _runner.ConnectionString,
                ["MongoDB:DatabaseName"] = "geolocalization_tests"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddApplication();
        services.AddInfraShared(configuration);

        ServiceProvider = services.BuildServiceProvider();

        await Task.CompletedTask;
    }

    public async Task ClearPartnerGeolocationsAsync()
    {
        using var scope = ServiceProvider.CreateScope();
        var database = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();
        await database
            .GetCollection<PartnerGeolocationDbModel>("partner_geolocations")
            .DeleteManyAsync(FilterDefinition<PartnerGeolocationDbModel>.Empty);
    }

    public Task DisposeAsync()
    {
        _runner?.Dispose();
        return Task.CompletedTask;
    }
}

[CollectionDefinition("mongo")]
public class MongoCollection : ICollectionFixture<MongoFixture> { }
