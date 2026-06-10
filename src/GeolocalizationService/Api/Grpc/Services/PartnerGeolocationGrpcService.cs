using GeolocalizationService.Api.Grpc.Mappers;
using GeolocalizationService.Application.PartnerGeolocation.UseCases;
using Grpc.Core;
using ZeDelivery.Contracts.Geolocation;

namespace GeolocalizationService.Api.Grpc.Services;

public sealed class PartnerGeolocationGrpcService : PartnerGeolocationService.PartnerGeolocationServiceBase
{
    private readonly IGetPartnerGeolocationByPartnerId _getByPartnerId;
    private readonly IGetNearestPartnerGeolocation _getNearest;

    public PartnerGeolocationGrpcService(
        IGetPartnerGeolocationByPartnerId getByPartnerId,
        IGetNearestPartnerGeolocation getNearest)
    {
        _getByPartnerId = getByPartnerId;
        _getNearest = getNearest;
    }

    public override async Task<PartnerGeolocationResponse> GetPartnerGeolocationByPartnerId(
        GetPartnerGeolocationByPartnerIdRequest request,
        ServerCallContext context)
    {
        var result = await _getByPartnerId.ExecuteAsync(request.ToGetByIdInput());

        if (result is null)
            throw new RpcException(new Status(StatusCode.NotFound, $"Partner geolocation '{request.PartnerId}' was not found."));

        return result.ToGrpcResponse();
    }

    public override async Task<PartnerGeolocationResponse> GetNearestPartnerGeolocation(
        GetNearestPartnerGeolocationRequest request,
        ServerCallContext context)
    {
        var result = await _getNearest.ExecuteAsync(request.ToNearestInput());

        if (result is null)
            throw new RpcException(new Status(StatusCode.NotFound, "No partner found for the given location."));

        return result.ToGrpcResponse();
    }
}
