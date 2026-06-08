

using PartnerService.Application.GeoLocalization.DataTransferObjects;
using PartnerService.Application.GeoLocalization.Events;

namespace PartnerService.Application.GeoLocalization.Contracts;

public interface IGeolocalizationClient
{
    Task CreatePartnerGeolocalizationAsync(PartnerCreatedIntegrationEvent payload);
    Task<PartnerGeolocalizationDTO> GetPartnerGeolocalizationAsync(Guid partnerId);
}