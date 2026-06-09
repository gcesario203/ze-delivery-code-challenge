
using GeolocalizationService.Application.PartnerGeolocation.DataTransferObjects;

namespace GeolocalizationService.Application.PartnerGeolocation.UseCases;

public interface ICreatePartnerGeolocation
{
    Task<PartnerGeolocationViewModel> ExecuteAsync(CreatePartnerGeolocationInput input);
}