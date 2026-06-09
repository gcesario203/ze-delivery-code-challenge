namespace GeolocalizationService.Domain.PartnerGeolocation.Entities;

using GeolocalizationService.Domain.Shared.Entities;
using GeolocalizationService.Domain.Shared.ValueObjects;

public class PartnerGeolocationEntity : BaseEntity
{
    public Guid PartnerId => Id;

    public AddressVO Address { get; private set; }

    public CoverageAreaVO CoverageArea { get; private set; }

    private PartnerGeolocationEntity() : base()
    {
    }

    public PartnerGeolocationEntity(Guid partnerId, AddressVO address, CoverageAreaVO coverageArea)
        : base(partnerId)
    {
        SetGeoData(address, coverageArea);
    }

    public static PartnerGeolocationEntity Create(Guid partnerId, AddressVO address, CoverageAreaVO coverageArea)
    {
        var partnerGeolocation = new PartnerGeolocationEntity(partnerId, address, coverageArea);
        return partnerGeolocation;
    }

    public void Update(AddressVO address, CoverageAreaVO coverageArea) =>
        SetGeoData(address, coverageArea);

    private void SetGeoData(AddressVO address, CoverageAreaVO coverageArea)
    {
        if (address is null)
            throw new ArgumentException("Address is required.", nameof(address));

        if (coverageArea is null)
            throw new ArgumentException("Coverage area is required.", nameof(coverageArea));

        Address = address;
        CoverageArea = coverageArea;
    }
}
