
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PartnerService.Application.GeoLocalization.Events;
using PartnerService.Application.Shared.Contracts;
using PartnerService.Infra.Shared.Outbox.Enums;
using PartnerService.Infra.Shared.Persistence;
using PartnerService.Tests.Fixtures;

namespace PartnerService.Tests.UnitTests.Infra.Shared.Outbox;

[Collection("db")]
public class OutboxPublisherUnitTests
{
    private readonly InMemoryFixture _fixture;

    public OutboxPublisherUnitTests(InMemoryFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task EnqueueAsync_ShouldPersistOutboxMessage()
    {
        using var scope = _fixture.ServiceProvider.CreateScope();
        var publisher = scope.ServiceProvider.GetRequiredService<IOutboxPublisher>();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var integrationEvent = new PartnerCreatedIntegrationEvent(
            Guid.NewGuid(),
            PartnerTestData.ValidAddress,
            PartnerTestData.ValidCoverageArea);

        await publisher.EnqueueAsync(integrationEvent);

        var message = await context.OutboxMessages
            .OrderByDescending(item => item.CreatedAt)
            .FirstAsync(item => item.Payload.Contains(integrationEvent.PartnerId.ToString()));
        message.EventType.Should().Contain(nameof(PartnerCreatedIntegrationEvent));
        message.Status.Should().Be(OutboxMessageStatus.Pending);
        message.Payload.Should().Contain(integrationEvent.PartnerId.ToString());
    }
}
