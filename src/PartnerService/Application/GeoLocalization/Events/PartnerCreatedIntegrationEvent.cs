
using PartnerService.Application.Shared.DataTransferObjects;

namespace PartnerService.Application.GeoLocalization.Events;

public sealed class PartnerCreatedIntegrationEvent
{
    public Guid PartnerId { get; }

    public AddressDTO Address { get; set; }

    public CoverageAreaDTO CoverageArea { get; set; }

    public PartnerCreatedIntegrationEvent(Guid partnerId, AddressDTO address, CoverageAreaDTO coverageArea)
    {
        PartnerId = partnerId;
        Address = address;
        CoverageArea = coverageArea;
    }
}