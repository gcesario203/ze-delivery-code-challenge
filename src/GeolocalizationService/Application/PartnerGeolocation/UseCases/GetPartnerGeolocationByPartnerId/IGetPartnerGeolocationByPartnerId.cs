
using GeolocalizationService.Application.PartnerGeolocation.DataTransferObjects;

namespace GeolocalizationService.Application.PartnerGeolocation.UseCases;

public interface IGetPartnerGeolocationByPartnerId
{
    Task<PartnerGeolocationViewModel> ExecuteAsync(GetPartnerGeolocationByPartnerIdInput input);
}
