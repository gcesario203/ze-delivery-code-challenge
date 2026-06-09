
using GeolocalizationService.Domain.PartnerGeolocation.Entities;
using GeolocalizationService.Domain.PartnerGeolocation.Repositories;
using GeolocalizationService.Domain.Shared.ValueObjects;
using GeolocalizationService.Infra.PartnerGeolocation.Mappers;
using GeolocalizationService.Infra.PartnerGeolocation.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;

namespace GeolocalizationService.Infra.PartnerGeolocation.Persistence;

public class PartnerGeolocationRepository : IPartnerGeolocationRepository
{
    private readonly IMongoCollection<PartnerGeolocationDbModel> _collection;

    public PartnerGeolocationRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<PartnerGeolocationDbModel>("partner_geolocations");
        EnsureIndexes();
    }

    private void EnsureIndexes()
    {
        var addressIndex = Builders<PartnerGeolocationDbModel>.IndexKeys
            .Geo2DSphere(x => x.Address);

        var coverageIndex = Builders<PartnerGeolocationDbModel>.IndexKeys
            .Geo2DSphere(x => x.CoverageArea);

        _collection.Indexes.CreateMany(new[]
        {
            new CreateIndexModel<PartnerGeolocationDbModel>(addressIndex),
            new CreateIndexModel<PartnerGeolocationDbModel>(coverageIndex)
        });
    }

    public async Task AddAsync(PartnerGeolocationEntity entity)
    {
        var exists = await _collection
            .Find(x => x.Id == entity.Id)
            .AnyAsync();

        if (exists)
            throw new InvalidOperationException($"Partner geolocation with id {entity.Id} already exists.");

        var dbModel = entity.ToDbModel();
        dbModel.CreatedAt = DateTime.UtcNow;

        await _collection.InsertOneAsync(dbModel);
    }

    public async Task<PartnerGeolocationEntity> GetByPartnerIdAsync(Guid partnerId)
    {
        var dbModel = await _collection
            .Find(x => x.Id == partnerId)
            .FirstOrDefaultAsync();

        return dbModel?.ToDomainEntity();
    }

    public async Task UpdateAsync(PartnerGeolocationEntity entity)
    {
        var existing = await _collection
            .Find(x => x.Id == entity.Id)
            .FirstOrDefaultAsync();

        if (existing is null)
            throw new KeyNotFoundException($"Partner geolocation with id {entity.Id} was not found.");

        var dbModel = entity.ToDbModel();
        dbModel.CreatedAt = existing.CreatedAt;
        dbModel.UpdatedAt = DateTime.UtcNow;

        var result = await _collection.ReplaceOneAsync(x => x.Id == entity.Id, dbModel);

        if (result.MatchedCount == 0)
            throw new KeyNotFoundException($"Partner geolocation with id {entity.Id} was not found.");
    }

    public async Task<IEnumerable<PartnerGeolocationEntity>> GetByGeolocationAsync(CordinateVO cordinate)
    {
        var geoResults = await TryFindByCoverageWithGeoNearAsync(cordinate);

        var eligiblePartners = geoResults is { Count: > 0 }
            ? geoResults
            : await FindByCoverageInMemoryAsync(cordinate);

        return eligiblePartners
            .OrderBy(partner => CalculateDistanceInMeters(
                cordinate.Latitude,
                cordinate.Longitude,
                partner.Address.Cordinates.Latitude,
                partner.Address.Cordinates.Longitude))
            .ToList();
    }

    private async Task<List<PartnerGeolocationEntity>> TryFindByCoverageWithGeoNearAsync(CordinateVO cordinate)
    {
        try
        {
            var pointBson = new BsonDocument
            {
                { "type", "Point" },
                { "coordinates", new BsonArray { cordinate.Longitude, cordinate.Latitude } }
            };

            var geoNearStage = new BsonDocument("$geoNear", new BsonDocument
            {
                { "near", pointBson },
                { "distanceField", "distance" },
                { "spherical", true },
                { "key", "address" },
                {
                    "query", new BsonDocument("coverageArea", new BsonDocument("$geoIntersects",
                        new BsonDocument("$geometry", pointBson)))
                }
            });

            var pipeline = PipelineDefinition<PartnerGeolocationDbModel, PartnerGeolocationDbModel>
                .Create(new[] { geoNearStage });

            var results = await _collection.Aggregate(pipeline).ToListAsync();

            return results.Select(model => model.ToDomainEntity()).ToList();
        }
        catch (MongoCommandException)
        {
            return null;
        }
    }

    private async Task<List<PartnerGeolocationEntity>> FindByCoverageInMemoryAsync(CordinateVO cordinate)
    {
        var allPartners = await _collection
            .Find(FilterDefinition<PartnerGeolocationDbModel>.Empty)
            .ToListAsync();

        return allPartners
            .Select(model => model.ToDomainEntity())
            .Where(partner => partner.CoverageArea.Contains(cordinate))
            .ToList();
    }

    private static double CalculateDistanceInMeters(
        double originLatitude,
        double originLongitude,
        double destinationLatitude,
        double destinationLongitude)
    {
        const double earthRadiusMeters = 6371000;

        var latitudeDelta = DegreesToRadians(destinationLatitude - originLatitude);
        var longitudeDelta = DegreesToRadians(destinationLongitude - originLongitude);

        var haversine =
            Math.Sin(latitudeDelta / 2) * Math.Sin(latitudeDelta / 2) +
            Math.Cos(DegreesToRadians(originLatitude)) * Math.Cos(DegreesToRadians(destinationLatitude)) *
            Math.Sin(longitudeDelta / 2) * Math.Sin(longitudeDelta / 2);

        var centralAngle = 2 * Math.Atan2(Math.Sqrt(haversine), Math.Sqrt(1 - haversine));

        return earthRadiusMeters * centralAngle;
    }

    private static double DegreesToRadians(double degrees) => degrees * Math.PI / 180;
}
