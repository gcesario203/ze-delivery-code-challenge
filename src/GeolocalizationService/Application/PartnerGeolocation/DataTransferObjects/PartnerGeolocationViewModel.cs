
using GeolocalizationService.Application.Shared.DataTransferObjects;

namespace GeolocalizationService.Application.PartnerGeolocation.DataTransferObjects;

public class PartnerGeolocationViewModel
{
    public Guid Id { get; set; }
    public AddressDTO Address { get; set; }
    public CoverageAreaDTO CoverageArea { get; set; }
}