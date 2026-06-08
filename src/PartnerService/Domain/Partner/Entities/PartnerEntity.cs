using PartnerService.Domain.Partner.Events;
using PartnerService.Domain.Shared.Entities;
using PartnerService.Domain.Shared.ValueObjects;

namespace PartnerService.Domain.Partner.Entities;

public class PartnerEntity : BaseEntity
{
    public string TradingName { get; private set; }

    public string OwnerName { get; private set; }
    public CnpjVO Document { get; private set; }

    public PartnerEntity() : base()
    {
    }

    public PartnerEntity(Guid id, string tradingName, string ownerName, CnpjVO document)
    : base(id)
    {
        Create(tradingName, ownerName, document);
    }

    public PartnerEntity(string tradingName, string ownerName, CnpjVO document)
    : base()
    {
        Create(tradingName, ownerName, document);
    }

    public static PartnerEntity CreatePartner(string tradingName,
                                       string ownerName,
                                       CnpjVO document,
                                       AddressVO address,
                                       CoverageAreaVO coverageArea)
    {
        var partner = new PartnerEntity(tradingName, ownerName, document);
        partner.AddDomainEvent(new PartnerCreatedEvent(partner.Id, address, coverageArea));
        return partner;
    }

    private void Create(string tradingName, string ownerName, CnpjVO document)
    {
        if (string.IsNullOrEmpty(tradingName))
            throw new ArgumentException("Trading name is required.");

        if (string.IsNullOrEmpty(ownerName))
            throw new ArgumentException("Owner name is required.");

        if (document == null)
            throw new ArgumentException("Document is required.");

        TradingName = tradingName;
        OwnerName = ownerName;
        Document = document;
    }

    public void UpdateTradingName(string tradingName)
    {
        if (string.IsNullOrEmpty(tradingName))
            throw new ArgumentException("Trading name is required.");

        TradingName = tradingName;
    }

    public void UpdateOwnerName(string ownerName)
    {
        if (string.IsNullOrEmpty(ownerName))
            throw new ArgumentException("Owner name is required.");

        OwnerName = ownerName;
    }

    public void UpdateDocument(CnpjVO document)
    {
        if (document == null)
            throw new ArgumentException("Document is required.");

        Document = document;
    }
}