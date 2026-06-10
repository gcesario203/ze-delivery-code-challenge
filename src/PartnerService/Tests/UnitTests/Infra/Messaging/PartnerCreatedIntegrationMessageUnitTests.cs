using FluentAssertions;
using PartnerService.Application.GeoLocalization.Events;
using PartnerService.Tests.Fixtures;
using System.Text.Json;
using ZeDelivery.Contracts.Messaging;

namespace PartnerService.Tests.UnitTests.Infra.Messaging;

public class PartnerCreatedIntegrationMessageUnitTests
{
    [Fact]
    public void Serialize_ShouldRoundTripPartnerCreatedIntegrationMessage()
    {
        var partnerId = Guid.NewGuid();
        var integrationEvent = new PartnerCreatedIntegrationEvent(
            partnerId,
            PartnerTestData.ValidAddress,
            PartnerTestData.ValidCoverageArea);

        var message = new PartnerCreatedIntegrationMessage
        {
            PartnerId = integrationEvent.PartnerId,
            Address = new AddressMessage
            {
                Cordinates = new CoordinateMessage
                {
                    Latitude = integrationEvent.Address.Cordinates.Latitude,
                    Longitude = integrationEvent.Address.Cordinates.Longitude
                }
            },
            CoverageArea = new CoverageAreaMessage
            {
                Cordinates = integrationEvent.CoverageArea.Cordinates
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

        var json = JsonSerializer.Serialize(message);
        var deserialized = JsonSerializer.Deserialize<PartnerCreatedIntegrationMessage>(json);

        deserialized.Should().NotBeNull();
        deserialized.PartnerId.Should().Be(partnerId);
        deserialized.Address.Cordinates.Latitude.Should().Be(PartnerTestData.ValidCoordinate.Latitude);
        deserialized.CoverageArea.Cordinates.Should().HaveCount(1);
    }
}
