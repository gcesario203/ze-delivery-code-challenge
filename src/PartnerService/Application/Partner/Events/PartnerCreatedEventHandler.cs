
using Microsoft.Extensions.Logging;
using PartnerService.Application.GeoLocalization.Events;
using PartnerService.Application.Shared.Contracts;
using PartnerService.Application.Shared.Mappers;
using PartnerService.Domain.Partner.Events;

namespace PartnerService.Application.Partner.Events;

public sealed class PartnerCreatedEventHandler : IEventHandler<PartnerCreatedEvent>
{
    private readonly ILogger<PartnerCreatedEventHandler> _logger;

    private readonly IOutboxPublisher _outboxPublisher;

    public PartnerCreatedEventHandler(ILogger<PartnerCreatedEventHandler> logger, IOutboxPublisher outboxPublisher)
    {
        _logger = logger;
        _outboxPublisher = outboxPublisher;
    }

    public async Task Handle(PartnerCreatedEvent @event)
    {
        _logger.LogInformation(
            "Partner created: {PartnerId} - {Address} - {CoverageArea}",
            @event.PartnerId,
            @event.Address,
            @event.CoverageArea
        );

        var integrationEvent = new PartnerCreatedIntegrationEvent(
            @event.PartnerId,
            @event.Address.ToDTO(),
            @event.CoverageArea.ToDTO()
        );

        await _outboxPublisher.EnqueueAsync(integrationEvent);
    }
}