

using GeolocalizationService.Application.Shared.DataTransferObjects;

namespace GeolocalizationService.Application.PartnerGeolocation.UseCases;

public class CreatePartnerGeolocationInput
{
    public Guid Id { get; set; }
    public AddressDTO Address { get; set; }
    public CoverageAreaDTO CoverageArea { get; set; }
}