

using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PartnerService.Infra.Shared;
using PartnerService.Infra.Shared.Persistence;

namespace PartnerService.Tests.Fixtures;

public class InMemoryFixture : IAsyncLifetime
{
    public IServiceProvider ServiceProvider { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddInfraShared(new ConfigurationBuilder().Build(), useInMemoryDatabase: true);

        ServiceProvider = services.BuildServiceProvider();

        var db = ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        var db = ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.EnsureDeletedAsync();

        // Fecha a conexão âncora
        var conn = ServiceProvider.GetRequiredService<IDbConnection>();
        conn.Dispose();
    }
}

[CollectionDefinition("db")]
public class DbCollection : ICollectionFixture<InMemoryFixture> { }