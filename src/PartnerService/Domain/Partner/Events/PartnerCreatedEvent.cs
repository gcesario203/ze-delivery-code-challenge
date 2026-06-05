
using PartnerService.Domain.Shared.Events;

namespace PartnerService.Domain.Partner.Events;

public sealed class PartnerCreatedEvent : DomainEvent
{
    public Guid PartnerId { get; }

    public string TradingName { get; }

    public string OwnerName { get; }

    public string Document { get; }

    public PartnerCreatedEvent(Guid partnerId, string tradingName, string ownerName, string document)
    {
        PartnerId = partnerId;
        TradingName = tradingName;
        OwnerName = ownerName;
        Document = document;
    }
}