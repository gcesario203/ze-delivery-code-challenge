
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PartnerService.Application.GeoLocalization.Contracts;
using PartnerService.Application.GeoLocalization.Events;
using PartnerService.Application.Partner.Commands.CreatePartner;
using PartnerService.Application.Shared.Contracts;
using PartnerService.Domain.Partner.Repositories;
using PartnerService.Infra.Shared.Outbox.Enums;
using PartnerService.Infra.Shared.Persistence;
using PartnerService.Tests.Fixtures;

namespace PartnerService.Tests.UnitTests.Infra.Shared.Outbox;

[Collection("db")]
public class OutboxProcessorUnitTests
{
    private readonly InMemoryFixture _fixture;

    public OutboxProcessorUnitTests(InMemoryFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ProcessPendingAsync_ShouldPublishToGeolocalizationClient()
    {
        var partnerId = Guid.NewGuid();

        using (var scope = _fixture.ServiceProvider.CreateScope())
        {
            var publisher = scope.ServiceProvider.GetRequiredService<IOutboxPublisher>();
            await publisher.EnqueueAsync(new PartnerCreatedIntegrationEvent(
                partnerId,
                PartnerTestData.ValidAddress,
                PartnerTestData.ValidCoverageArea));
        }

        using (var scope = _fixture.ServiceProvider.CreateScope())
        {
            var processor = scope.ServiceProvider.GetRequiredService<IOutboxProcessor>();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var geoClient = scope.ServiceProvider.GetRequiredService<IGeolocalizationClient>();

            await processor.ProcessPendingAsync();

            var message = await context.OutboxMessages
                .SingleAsync(item => item.Id != Guid.Empty && item.Payload.Contains(partnerId.ToString()));
            message.Status.Should().Be(OutboxMessageStatus.Processed);
            message.ProcessedAt.Should().NotBeNull();

            var geolocalization = await geoClient.GetPartnerGeolocalizationAsync(partnerId);
            geolocalization.Should().NotBeNull();
            geolocalization.Address.Should().NotBeNull();
            geolocalization.CoverageArea.Should().NotBeNull();
        }
    }
}
