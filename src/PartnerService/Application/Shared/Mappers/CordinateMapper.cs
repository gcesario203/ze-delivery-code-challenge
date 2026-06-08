
using PartnerService.Application.Shared.DataTransferObjects;
using PartnerService.Domain.Shared.ValueObjects;

namespace PartnerService.Application.Shared.Mappers;

public static class CordinateMapper
{
    public static IReadOnlyList<IReadOnlyList<IReadOnlyList<CordinateDTO>>> ToDTO(
        this IReadOnlyList<IReadOnlyList<IReadOnlyList<CordinateVO>>> cordinates)
        => cordinates.Select(c1 => c1.Select(c2 => c2.Select(c3 => c3.ToDTO()).ToList()).ToList()).ToList();

    public static IReadOnlyList<IReadOnlyList<IReadOnlyList<CordinateVO>>> ToVO(
        this IReadOnlyList<IReadOnlyList<IReadOnlyList<CordinateDTO>>> cordinates)
        => cordinates.Select(c1 => c1.Select(c2 => c2.Select(c3 => c3.ToVO()).ToList()).ToList()).ToList();

    public static CordinateDTO ToDTO(this CordinateVO cordinate)
        => new CordinateDTO
        {
            Latitude = cordinate.Latitude,
            Longitude = cordinate.Longitude
        };

    public static CordinateVO ToVO(this CordinateDTO cordinate)
        => new CordinateVO(cordinate.Latitude, cordinate.Longitude);
}