
using GeolocalizationService.Application.Shared.DataTransferObjects;

namespace GeolocalizationService.Application.PartnerGeolocation.UseCases;

public sealed class GetNearestPartnerGeolocationInput
{
    public CordinateDTO Cordinates { get; set; }
}
