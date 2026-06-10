using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PartnerService.Application.Shared;
using PartnerService.Infra.Shared;
using PartnerService.Infra.Shared.Persistence;

namespace PartnerService.Tests.Fixtures;

public class InMemoryFixture : IAsyncLifetime
{
    public IServiceProvider ServiceProvider { get; private set; } = null!;
    private IHost _host = null!;

    public async Task InitializeAsync()
    {
        _host = await Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
            {
                services.AddLogging();
                services.AddInfraShared(
                    new ConfigurationBuilder().Build(),
                    useInMemoryDatabase: true,
                    useInMemoryGeolocalization: true);
                services.AddApplication();
            })
            .AddWolwerine()  // IHostBuilder extension
            .StartAsync();

        ServiceProvider = _host.Services;

        using var scope = ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        using var scope = ServiceProvider.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.EnsureDeletedAsync();

        var conn = scope.ServiceProvider.GetRequiredService<IDbConnection>();
        conn.Dispose();

        await _host.StopAsync();
        _host.Dispose();
    }
}

[CollectionDefinition("db")]
public class DbCollection : ICollectionFixture<InMemoryFixture> { }