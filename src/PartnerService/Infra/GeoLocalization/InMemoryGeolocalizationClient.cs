
using System.Collections.Concurrent;
using PartnerService.Application.GeoLocalization.Contracts;
using PartnerService.Application.GeoLocalization.DataTransferObjects;
using PartnerService.Application.GeoLocalization.Events;

namespace PartnerService.Infra.GeoLocalization;

public sealed class InMemoryGeolocalizationClient : IGeolocalizationClient
{
    private readonly ConcurrentDictionary<Guid, PartnerGeolocalizationDTO> _store = new();

    public Task CreatePartnerGeolocalizationAsync(PartnerCreatedIntegrationEvent payload)
    {
        _store[payload.PartnerId] = new PartnerGeolocalizationDTO
        {
            Address = payload.Address,
            CoverageArea = payload.CoverageArea
        };

        return Task.CompletedTask;
    }

    public Task<PartnerGeolocalizationDTO> GetPartnerGeolocalizationAsync(Guid partnerId)
    {
        _store.TryGetValue(partnerId, out var geolocalization);
        return Task.FromResult(geolocalization);
    }
}
