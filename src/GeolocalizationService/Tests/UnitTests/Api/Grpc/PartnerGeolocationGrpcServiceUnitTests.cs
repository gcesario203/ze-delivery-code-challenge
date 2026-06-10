using FluentAssertions;
using GeolocalizationService.Api.Grpc.Services;
using GeolocalizationService.Application.PartnerGeolocation.DataTransferObjects;
using GeolocalizationService.Application.PartnerGeolocation.UseCases;
using GeolocalizationService.Tests.Fixtures;
using Grpc.Core;
using NSubstitute;
using ZeDelivery.Contracts.Geolocation;

namespace GeolocalizationService.Tests.UnitTests.Api.Grpc;

public class PartnerGeolocationGrpcServiceUnitTests
{
    [Fact]
    public async Task GetNearestPartnerGeolocation_ShouldReturnMappedResponse()
    {
        var partnerId = Guid.NewGuid();
        var getNearest = Substitute.For<IGetNearestPartnerGeolocation>();
        getNearest.ExecuteAsync(Arg.Any<GetNearestPartnerGeolocationInput>())
            .Returns(new PartnerGeolocationViewModel
            {
                Id = partnerId,
                Address = GeolocationTestData.ValidAddress,
                CoverageArea = GeolocationTestData.ValidCoverageArea
            });

        var service = new PartnerGeolocationGrpcService(Substitute.For<IGetPartnerGeolocationByPartnerId>(), getNearest);

        var response = await service.GetNearestPartnerGeolocation(
            new GetNearestPartnerGeolocationRequest
            {
                Coordinates = new Coordinate { Latitude = -23.551, Longitude = -46.634 }
            },
            new TestServerCallContext());

        response.PartnerId.Should().Be(partnerId.ToString());
    }

    [Fact]
    public async Task GetNearestPartnerGeolocation_ShouldThrowNotFound_WhenNoPartnerExists()
    {
        var getNearest = Substitute.For<IGetNearestPartnerGeolocation>();
        getNearest.ExecuteAsync(Arg.Any<GetNearestPartnerGeolocationInput>()).Returns((PartnerGeolocationViewModel)null);

        var service = new PartnerGeolocationGrpcService(Substitute.For<IGetPartnerGeolocationByPartnerId>(), getNearest);

        var act = () => service.GetNearestPartnerGeolocation(
            new GetNearestPartnerGeolocationRequest
            {
                Coordinates = new Coordinate { Latitude = -23.551, Longitude = -46.634 }
            },
            new TestServerCallContext());

        var exception = await act.Should().ThrowAsync<RpcException>();
        exception.Which.StatusCode.Should().Be(StatusCode.NotFound);
    }

    private sealed class TestServerCallContext : ServerCallContext
    {
        protected override Task WriteResponseHeadersAsyncCore(Metadata responseHeaders) => Task.CompletedTask;

        protected override ContextPropagationToken CreatePropagationTokenCore(ContextPropagationOptions options) => null;

        protected override string MethodCore => "test";

        protected override string HostCore => "localhost";

        protected override string PeerCore => "peer";

        protected override DateTime DeadlineCore => DateTime.UtcNow.AddMinutes(1);

        protected override Metadata RequestHeadersCore => new();

        protected override CancellationToken CancellationTokenCore => CancellationToken.None;

        protected override Metadata ResponseTrailersCore => new();

        protected override Status StatusCore { get; set; }

        protected override WriteOptions WriteOptionsCore { get; set; }

        protected override AuthContext AuthContextCore => new(string.Empty, new Dictionary<string, List<AuthProperty>>());
    }
}
