

namespace GeolocalizationService.Domain.PartnerGeolocation.Repositories;

using GeolocalizationService.Domain.PartnerGeolocation.Entities;
using GeolocalizationService.Domain.Shared.ValueObjects;

public interface IPartnerGeolocationRepository
{
    Task<PartnerGeolocationEntity> GetByPartnerIdAsync(Guid partnerId);

    Task AddAsync(PartnerGeolocationEntity entity);

    Task UpdateAsync(PartnerGeolocationEntity entity);

    /// <summary>
    /// Returns partners whose coverage area contains the given point, ordered by distance to their address (nearest first).
    /// </summary>
    Task<IEnumerable<PartnerGeolocationEntity>> GetByGeolocationAsync(CordinateVO cordinate);
}