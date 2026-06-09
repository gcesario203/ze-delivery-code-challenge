
using GeolocalizationService.Application.PartnerGeolocation.DataTransferObjects;

namespace GeolocalizationService.Application.PartnerGeolocation.UseCases;

public interface IGetNearestPartnerGeolocation
{
    Task<PartnerGeolocationViewModel> ExecuteAsync(GetNearestPartnerGeolocationInput input);
}
