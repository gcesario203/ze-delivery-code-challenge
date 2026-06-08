
using PartnerService.Domain.Shared.Enums;

namespace PartnerService.Domain.Shared.ValueObjects;

public sealed class CoverageAreaVO
{
    public CordinateTypeVO Type { get; } = CordinateTypeVO.MultiPolygon;
    public IReadOnlyList<IReadOnlyList<IReadOnlyList<CordinateVO>>> Cordinates { get; }

    public bool IsEmpty => !Cordinates.Any();

    public CoverageAreaVO(List<List<List<CordinateVO>>> coordinates)
    {
        if(coordinates == null || !coordinates.Any())
            throw new ArgumentException("Coverage area coordinates cannot be null or empty.", nameof(coordinates));
        Cordinates = coordinates;
    }
}