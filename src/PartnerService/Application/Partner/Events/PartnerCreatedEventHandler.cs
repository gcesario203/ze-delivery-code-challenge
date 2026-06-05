
using Microsoft.Extensions.Logging;
using PartnerService.Application.Shared.Contracts;
using PartnerService.Domain.Partner.Events;

namespace PartnerService.Application.Partner.Events;

public sealed class PartnerCreatedEventHandler : IEventHandler<PartnerCreatedEvent>
{
    private readonly ILogger<PartnerCreatedEventHandler> _logger;

    public PartnerCreatedEventHandler(ILogger<PartnerCreatedEventHandler> logger)
        => _logger = logger;

    public Task Handle(PartnerCreatedEvent @event)
    {
        _logger.LogInformation(
            "Partner created: {PartnerId} - {TradingName} - {OwnerName} - {Document}",
            @event.PartnerId,
            @event.TradingName,
            @event.OwnerName,
            @event.Document
        );

        return Task.CompletedTask;
    }
}