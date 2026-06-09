
using GeolocalizationService.Application.Shared.DataTransferObjects;

namespace GeolocalizationService.Application.PartnerGeolocation.UseCases;

public sealed class UpdatePartnerGeolocationInput
{
    public Guid Id { get; set; }
    public AddressDTO Address { get; set; }
    public CoverageAreaDTO CoverageArea { get; set; }
}
