using GeolocalizationService.Application.PartnerGeolocation.DataTransferObjects;
using GeolocalizationService.Application.PartnerGeolocation.UseCases;
using GeolocalizationService.Application.Shared.DataTransferObjects;
using ZeDelivery.Contracts.Geolocation;

namespace GeolocalizationService.Api.Grpc.Mappers;

public static class GrpcGeolocationMapper
{
    public static PartnerGeolocationResponse ToGrpcResponse(this PartnerGeolocationViewModel viewModel)
    {
        var response = new PartnerGeolocationResponse
        {
            PartnerId = viewModel.Id.ToString(),
            Address = new Address
            {
                Coordinates = new Coordinate
                {
                    Latitude = viewModel.Address.Cordinates.Latitude,
                    Longitude = viewModel.Address.Cordinates.Longitude
                }
            },
            CoverageArea = new CoverageArea()
        };

        foreach (var polygon in viewModel.CoverageArea.Cordinates)
        {
            var grpcPolygon = new Polygon();

            foreach (var ring in polygon)
            {
                var grpcRing = new LinearRing();
                grpcRing.Coordinates.AddRange(ring.Select(coordinate => new Coordinate
                {
                    Latitude = coordinate.Latitude,
                    Longitude = coordinate.Longitude
                }));
                grpcPolygon.Rings.Add(grpcRing);
            }

            response.CoverageArea.Polygons.Add(grpcPolygon);
        }

        return response;
    }

    public static GetNearestPartnerGeolocationInput ToNearestInput(this GetNearestPartnerGeolocationRequest request)
        => new()
        {
            Cordinates = new CordinateDTO
            {
                Latitude = request.Coordinates.Latitude,
                Longitude = request.Coordinates.Longitude
            }
        };

    public static GetPartnerGeolocationByPartnerIdInput ToGetByIdInput(this GetPartnerGeolocationByPartnerIdRequest request)
        => new()
        {
            PartnerId = Guid.Parse(request.PartnerId)
        };

    public static CreatePartnerGeolocationInput ToCreateInput(this ZeDelivery.Contracts.Messaging.PartnerCreatedIntegrationMessage message)
        => new()
        {
            Id = message.PartnerId,
            Address = new AddressDTO
            {
                Cordinates = new CordinateDTO
                {
                    Latitude = message.Address.Cordinates.Latitude,
                    Longitude = message.Address.Cordinates.Longitude
                }
            },
            CoverageArea = new CoverageAreaDTO
            {
                Cordinates = message.CoverageArea.Cordinates
                    .Select(polygon => polygon
                        .Select(ring => ring
                            .Select(coordinate => new CordinateDTO
                            {
                                Latitude = coordinate.Latitude,
                                Longitude = coordinate.Longitude
                            })
                            .ToList())
                        .ToList())
                    .ToList()
            }
        };
}
