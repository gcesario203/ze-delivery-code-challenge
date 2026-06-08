
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PartnerService.Application.GeoLocalization.Events;
using PartnerService.Application.Partner.Commands.CreatePartner;
using PartnerService.Application.Shared.Contracts;
using PartnerService.Domain.Partner.Repositories;
using PartnerService.Infra.Shared.Outbox.Enums;
using PartnerService.Infra.Shared.Persistence;
using PartnerService.Tests.Fixtures;

namespace PartnerService.Tests.Integration.Shared.Events;

[Collection("db")]
public class DomainEventsDispatcherIntegrationTests
{
    private readonly InMemoryFixture _fixture;

    public DomainEventsDispatcherIntegrationTests(InMemoryFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Dispatch_ShouldPersistIntegrationEventInOutbox_WhenPartnerIsCreated()
    {
        using var scope = _fixture.ServiceProvider.CreateScope();

        var commandHandler = new CreatePartnerCommandHandler(
            scope.ServiceProvider.GetRequiredService<IPartnerCommandRepository>(),
            scope.ServiceProvider.GetRequiredService<IUnitOfWork>(),
            new CreatePartnerCommandValidator(),
            scope.ServiceProvider.GetRequiredService<IPartnerQueryRepository>(),
            scope.ServiceProvider.GetRequiredService<ILogger<CreatePartnerCommandHandler>>());

        var createdPartner = await commandHandler.Handle(PartnerTestData.CreateValidCommand("53665844000118"));

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var outboxMessage = await db.OutboxMessages
            .OrderByDescending(message => message.CreatedAt)
            .FirstAsync(message => message.Payload.Contains(createdPartner.Id));

        outboxMessage.Status.Should().Be(OutboxMessageStatus.Pending);
        outboxMessage.EventType.Should().Contain(nameof(PartnerCreatedIntegrationEvent));
        outboxMessage.Payload.Should().Contain("PartnerId");
        outboxMessage.Payload.Should().Contain("Address");
    }
}
