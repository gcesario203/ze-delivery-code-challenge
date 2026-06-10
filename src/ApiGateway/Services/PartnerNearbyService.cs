using ApiGateway.Models;
using Grpc.Core;
using Grpc.Net.Client;
using ZeDelivery.Contracts.Geolocation;

namespace ApiGateway.Services;

public interface IPartnerNearbyService
{
    Task<PartnerNearbyResponse> GetNearestPartnerAsync(double latitude, double longitude, CancellationToken cancellationToken);
}

public sealed class PartnerNearbyService : IPartnerNearbyService, IDisposable
{
    private readonly PartnerGeolocationService.PartnerGeolocationServiceClient _geolocationClient;
    private readonly HttpClient _partnerHttpClient;
    private readonly GrpcChannel _grpcChannel;

    public PartnerNearbyService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

        var grpcAddress = configuration["Geolocalization:GrpcAddress"] ?? "http://localhost:5201";
        _grpcChannel = GrpcChannel.ForAddress(grpcAddress);
        _geolocationClient = new PartnerGeolocationService.PartnerGeolocationServiceClient(_grpcChannel);
        _partnerHttpClient = httpClientFactory.CreateClient("partner-service");
    }

    public async Task<PartnerNearbyResponse> GetNearestPartnerAsync(
        double latitude,
        double longitude,
        CancellationToken cancellationToken)
    {
        PartnerGeolocationResponse geolocation;

        try
        {
            geolocation = await _geolocationClient.GetNearestPartnerGeolocationAsync(
                new GetNearestPartnerGeolocationRequest
                {
                    Coordinates = new Coordinate
                    {
                        Latitude = latitude,
                        Longitude = longitude
                    }
                },
                cancellationToken: cancellationToken);
        }
        catch (RpcException exception) when (exception.StatusCode == StatusCode.NotFound)
        {
            return null;
        }

        var partnerResponse = await _partnerHttpClient.GetFromJsonAsync<ApiResponse<PartnerNearbyResponse>>(
            $"/partners/{geolocation.PartnerId}",
            cancellationToken);

        if (partnerResponse?.Data is null)
            return null;

        return new PartnerNearbyResponse
        {
            Id = partnerResponse.Data.Id,
            TradingName = partnerResponse.Data.TradingName,
            OwnerName = partnerResponse.Data.OwnerName,
            Document = partnerResponse.Data.Document,
            Address = ToAddressResponse(geolocation),
            CoverageArea = ToCoverageAreaResponse(geolocation)
        };
    }

    private static AddressResponse ToAddressResponse(PartnerGeolocationResponse geolocation)
        => new()
        {
            Cordinates = new CoordinateResponse
            {
                Latitude = geolocation.Address.Coordinates.Latitude,
                Longitude = geolocation.Address.Coordinates.Longitude
            }
        };

    private static CoverageAreaResponse ToCoverageAreaResponse(PartnerGeolocationResponse geolocation)
        => new()
        {
            Cordinates = geolocation.CoverageArea.Polygons
                .Select(polygon => polygon.Rings
                    .Select(ring => ring.Coordinates
                        .Select(coordinate => new CoordinateResponse
                        {
                            Latitude = coordinate.Latitude,
                            Longitude = coordinate.Longitude
                        })
                        .ToList())
                    .ToList())
                .ToList()
        };

    public void Dispose() => _grpcChannel.Dispose();
}
