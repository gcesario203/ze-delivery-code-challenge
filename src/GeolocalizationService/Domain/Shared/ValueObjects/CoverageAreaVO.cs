
using GeolocalizationService.Domain.Shared.Enums;

namespace GeolocalizationService.Domain.Shared.ValueObjects;

public sealed class CoverageAreaVO
{
    public CordinateTypeVO Type { get; } = CordinateTypeVO.MultiPolygon;
    public IReadOnlyList<IReadOnlyList<IReadOnlyList<CordinateVO>>> Cordinates { get; }

    public bool IsEmpty => !Cordinates.Any();

    public bool Contains(CordinateVO point)
    {
        if (point is null)
            throw new ArgumentException("Point is required.", nameof(point));

        return Cordinates.Any(polygon => IsPointInRing(point, polygon[0]));
    }

    public CoverageAreaVO(List<List<List<CordinateVO>>> coordinates)
    {
        if(coordinates == null || !coordinates.Any())
            throw new ArgumentException("Coverage area coordinates cannot be null or empty.", nameof(coordinates));
        Cordinates = coordinates;
    }

    private static bool IsPointInRing(CordinateVO point, IReadOnlyList<CordinateVO> ring)
    {
        var lastVertex = ring[ring.Count - 1];
        var vertices = ring.Count > 1 && ring[0].Equals(lastVertex)
            ? ring.Take(ring.Count - 1).ToList()
            : ring.ToList();

        var windingNumber = 0;

        for (var i = 0; i < vertices.Count; i++)
        {
            var current = vertices[i];
            var next = vertices[(i + 1) % vertices.Count];

            if (current.Latitude <= point.Latitude)
            {
                if (next.Latitude > point.Latitude && IsLeft(current, next, point) > 0)
                    windingNumber++;
            }
            else if (next.Latitude <= point.Latitude && IsLeft(current, next, point) < 0)
            {
                windingNumber--;
            }
        }

        return windingNumber != 0;
    }

    private static double IsLeft(CordinateVO start, CordinateVO end, CordinateVO point) =>
        (end.Longitude - start.Longitude) * (point.Latitude - start.Latitude) -
        (point.Longitude - start.Longitude) * (end.Latitude - start.Latitude);
}