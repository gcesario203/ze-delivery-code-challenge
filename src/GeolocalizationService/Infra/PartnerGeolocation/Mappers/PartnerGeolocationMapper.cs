
using System;
using GeolocalizationService.Domain.PartnerGeolocation.Entities;
using GeolocalizationService.Domain.Shared.ValueObjects;
using GeolocalizationService.Infra.PartnerGeolocation.Models;
using MongoDB.Driver.GeoJsonObjectModel;

namespace GeolocalizationService.Infra.PartnerGeolocation.Mappers;


public static class PartnerGeolocationMapper
{
    public static PartnerGeolocationEntity ToDomainEntity(this PartnerGeolocationDbModel dbModel)
    {
        return new PartnerGeolocationEntity(
            dbModel.Id,
            dbModel.Address.GeoLocationToAddress(),
            dbModel.CoverageArea.GeoLocationToCoverageArea()
        );
    }

    public static PartnerGeolocationDbModel ToDbModel(this PartnerGeolocationEntity entity)
    {
        return new PartnerGeolocationDbModel
        {
            Id = entity.Id,
            Address = entity.Address.AddressToGeoLocation(),
            CoverageArea = entity.CoverageArea.CoverageAreaToGeoLocation(),
            CreatedAt = DateTime.UtcNow
        };
    }

    public static GeoJsonPoint<GeoJson2DGeographicCoordinates> AddressToGeoLocation(this AddressVO address)
    {
        return new GeoJsonPoint<GeoJson2DGeographicCoordinates>(
            new GeoJson2DGeographicCoordinates(address.Cordinates.Longitude, address.Cordinates.Latitude));
    }

    public static GeoJsonMultiPolygon<GeoJson2DGeographicCoordinates> CoverageAreaToGeoLocation(this CoverageAreaVO coverageArea)
    {
        var polygonCoordinatesList = new List<GeoJsonPolygonCoordinates<GeoJson2DGeographicCoordinates>>();

        foreach (var polygon in coverageArea.Cordinates)
        {
            var exteriorRing = new GeoJsonLinearRingCoordinates<GeoJson2DGeographicCoordinates>(
                polygon[0].Select(coord => new GeoJson2DGeographicCoordinates(coord.Longitude, coord.Latitude)).ToList()
            );

            var holes = polygon.Skip(1).Select(ring =>
                new GeoJsonLinearRingCoordinates<GeoJson2DGeographicCoordinates>(
                    ring.Select(coord => new GeoJson2DGeographicCoordinates(coord.Longitude, coord.Latitude)).ToList()
                )
            ).ToList();

            var polygonCoordinates = new GeoJsonPolygonCoordinates<GeoJson2DGeographicCoordinates>(exteriorRing, holes);
            polygonCoordinatesList.Add(polygonCoordinates);
        }

        return new GeoJsonMultiPolygon<GeoJson2DGeographicCoordinates>(
            new GeoJsonMultiPolygonCoordinates<GeoJson2DGeographicCoordinates>(polygonCoordinatesList)
        );
    }

    public static AddressVO GeoLocationToAddress(this GeoJsonPoint<GeoJson2DGeographicCoordinates> geoLocation)
    {
        var coordinates = geoLocation.Coordinates;
        return new AddressVO(new CordinateVO(coordinates.Latitude, coordinates.Longitude));
    }

    public static CoverageAreaVO GeoLocationToCoverageArea(this GeoJsonMultiPolygon<GeoJson2DGeographicCoordinates> geoLocation)
    {
        var coordinates = new List<List<List<CordinateVO>>>();

        foreach (var polygon in geoLocation.Coordinates.Polygons)
        {
            var polygonCoordinates = new List<List<CordinateVO>>();

            var exteriorRing = polygon.Exterior.Positions
                .Select(coord => new CordinateVO(coord.Latitude, coord.Longitude))
                .ToList();
            polygonCoordinates.Add(exteriorRing);

            foreach (var hole in polygon.Holes)
            {
                var holeCoordinates = hole.Positions
                    .Select(coord => new CordinateVO(coord.Latitude, coord.Longitude))
                    .ToList();
                polygonCoordinates.Add(holeCoordinates);
            }

            coordinates.Add(polygonCoordinates);
        }

        return new CoverageAreaVO(coordinates);
    }
}