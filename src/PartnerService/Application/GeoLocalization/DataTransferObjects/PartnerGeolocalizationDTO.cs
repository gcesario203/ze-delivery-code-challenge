

using PartnerService.Application.Shared.DataTransferObjects;

namespace PartnerService.Application.GeoLocalization.DataTransferObjects;

public sealed class PartnerGeolocalizationDTO
{
    public AddressDTO Address { get; set; }

    public CoverageAreaDTO CoverageArea { get; set; }
}