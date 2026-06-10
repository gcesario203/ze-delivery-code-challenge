using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using PartnerService.Application.GeoLocalization.Contracts;
using PartnerService.Application.GeoLocalization.DataTransferObjects;
using PartnerService.Application.GeoLocalization.Events;
using PartnerService.Application.Shared.DataTransferObjects;
using PartnerService.Infra.Messaging;
using ZeDelivery.Contracts.Geolocation;

namespace PartnerService.Infra.GeoLocalization;

public sealed class GrpcGeolocalizationClient : IGeolocalizationClient, IDisposable
{
    private readonly IPartnerGeolocationPublisher _publisher;
    private readonly PartnerGeolocationService.PartnerGeolocationServiceClient _grpcClient;
    private readonly GrpcChannel _channel;

    public GrpcGeolocalizationClient(
        IPartnerGeolocationPublisher publisher,
        IConfiguration configuration)
    {
        AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

        _publisher = publisher;
        var grpcAddress = configuration["Geolocalization:GrpcAddress"] ?? "http://localhost:5201";
        _channel = GrpcChannel.ForAddress(grpcAddress);
        _grpcClient = new PartnerGeolocationService.PartnerGeolocationServiceClient(_channel);
    }

    public Task CreatePartnerGeolocalizationAsync(PartnerCreatedIntegrationEvent payload)
        => _publisher.PublishPartnerCreatedAsync(payload);

    public async Task<PartnerGeolocalizationDTO> GetPartnerGeolocalizationAsync(Guid partnerId)
    {
        try
        {
            var response = await _grpcClient.GetPartnerGeolocationByPartnerIdAsync(
                new GetPartnerGeolocationByPartnerIdRequest { PartnerId = partnerId.ToString() });

            return ToDto(response);
        }
        catch (RpcException exception) when (exception.StatusCode == StatusCode.NotFound)
        {
            return null;
        }
    }

    private static PartnerGeolocalizationDTO ToDto(PartnerGeolocationResponse response)
    {
        return new PartnerGeolocalizationDTO
        {
            Address = new AddressDTO
            {
                Cordinates = new CordinateDTO
                {
                    Latitude = response.Address.Coordinates.Latitude,
                    Longitude = response.Address.Coordinates.Longitude
                }
            },
            CoverageArea = new CoverageAreaDTO
            {
                Cordinates = response.CoverageArea.Polygons
                    .Select(polygon => polygon.Rings
                        .Select(ring => ring.Coordinates
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

    public void Dispose() => _channel.Dispose();
}
