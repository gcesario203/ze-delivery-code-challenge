
using GeolocalizationService.Application.PartnerGeolocation.DataTransferObjects;

namespace GeolocalizationService.Application.PartnerGeolocation.UseCases;

public interface IUpdatePartnerGeolocation
{
    Task<PartnerGeolocationViewModel> ExecuteAsync(UpdatePartnerGeolocationInput input);
}
