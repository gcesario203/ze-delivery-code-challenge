
using GeolocalizationService.Domain.PartnerGeolocation.Repositories;
using GeolocalizationService.Infra.PartnerGeolocation.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace GeolocalizationService.Infra.Shared.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddInfraShared(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MongoDB")
            ?? "mongodb://localhost:27017";

        var databaseName = configuration["MongoDB:DatabaseName"] ?? "geolocalization";

        services.AddSingleton<IMongoClient>(_ => new MongoClient(connectionString));
        services.AddScoped<IMongoDatabase>(sp =>
            sp.GetRequiredService<IMongoClient>().GetDatabase(databaseName));
        services.AddScoped<IPartnerGeolocationRepository, PartnerGeolocationRepository>();

        return services;
    }
}
