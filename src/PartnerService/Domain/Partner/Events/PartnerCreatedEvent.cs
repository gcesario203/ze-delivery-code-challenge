
using PartnerService.Domain.Shared.Events;
using PartnerService.Domain.Shared.ValueObjects;

namespace PartnerService.Domain.Partner.Events;

public sealed class PartnerCreatedEvent : DomainEvent
{
    public Guid PartnerId { get; }

    public AddressVO Address { get; }

    public CoverageAreaVO CoverageArea { get; }

    public PartnerCreatedEvent(Guid partnerId,
                               AddressVO address,
                               CoverageAreaVO coverageArea)
    {
        PartnerId = partnerId;
        Address = address;
        CoverageArea = coverageArea;
    }
}