
using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;

namespace GeolocalizationService.Infra.PartnerGeolocation.Models;

public class PartnerGeolocationDbModel
{
    [BsonId]
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid Id { get; set; }

    public GeoJsonPoint<GeoJson2DGeographicCoordinates> Address { get; set; }

    public GeoJsonMultiPolygon<GeoJson2DGeographicCoordinates> CoverageArea { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}