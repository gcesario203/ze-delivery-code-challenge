namespace GeolocalizationService.Domain.PartnerGeolocation.Entities;

using GeolocalizationService.Domain.Shared.Entities;
using GeolocalizationService.Domain.Shared.ValueObjects;

/// <summary>
/// Read model de geolocalização sincronizado a partir do PartnerService.
/// O <see cref="BaseEntity.Id"/> corresponde ao PartnerId de origem.
/// </summary>
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

    public static PartnerGeolocationEntity Create(Guid partnerId, AddressVO address, CoverageAreaVO coverageArea) =>
        new(partnerId, address, coverageArea);

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
