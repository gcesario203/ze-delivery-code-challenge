using FluentAssertions;
using GeolocalizationService.Api.Grpc.Mappers;
using GeolocalizationService.Application.PartnerGeolocation.DataTransferObjects;
using GeolocalizationService.Application.Shared.DataTransferObjects;
using GeolocalizationService.Tests.Fixtures;
using ZeDelivery.Contracts.Messaging;

namespace GeolocalizationService.Tests.UnitTests.Api.Grpc;

public class GrpcGeolocationMapperUnitTests
{
    [Fact]
    public void ToGrpcResponse_ShouldMapViewModel()
    {
        var viewModel = new PartnerGeolocationViewModel
        {
            Id = Guid.NewGuid(),
            Address = GeolocationTestData.ValidAddress,
            CoverageArea = GeolocationTestData.ValidCoverageArea
        };

        var response = viewModel.ToGrpcResponse();

        response.PartnerId.Should().Be(viewModel.Id.ToString());
        response.Address.Coordinates.Latitude.Should().Be(viewModel.Address.Cordinates.Latitude);
        response.CoverageArea.Polygons.Should().HaveCount(1);
    }

    [Fact]
    public void ToCreateInput_ShouldMapIntegrationMessage()
    {
        var message = new PartnerCreatedIntegrationMessage
        {
            PartnerId = Guid.NewGuid(),
            Address = new AddressMessage
            {
                Cordinates = new CoordinateMessage
                {
                    Latitude = -23.55052,
                    Longitude = -46.633308
                }
            },
            CoverageArea = new CoverageAreaMessage
            {
                Cordinates = GeolocationTestData.ValidCoverageArea.Cordinates
                    .Select(polygon => polygon
                        .Select(ring => ring
                            .Select(coordinate => new CoordinateMessage
                            {
                                Latitude = coordinate.Latitude,
                                Longitude = coordinate.Longitude
                            })
                            .ToList())
                        .ToList())
                    .ToList()
            }
        };

        var input = message.ToCreateInput();

        input.Id.Should().Be(message.PartnerId);
        input.Address.Cordinates.Latitude.Should().Be(-23.55052);
        input.CoverageArea.Cordinates.Should().HaveCount(1);
    }
}
