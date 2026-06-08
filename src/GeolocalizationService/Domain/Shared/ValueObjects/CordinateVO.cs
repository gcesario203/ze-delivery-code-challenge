
namespace GeolocalizationService.Domain.Shared.ValueObjects;

public class CordinateVO : IEquatable<CordinateVO>
{
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }

    public CordinateVO(double latitude, double longitude)
    {
        if (latitude < -90 || latitude > 90)
            throw new ArgumentOutOfRangeException(nameof(latitude), "Latitude must be between -90 and 90.");

        if (longitude < -180 || longitude > 180)
            throw new ArgumentOutOfRangeException(nameof(longitude), "Longitude must be between -180 and 180.");

        Latitude = latitude;
        Longitude = longitude;
    }

    public bool Equals(CordinateVO other) =>
        other is not null && Latitude == other.Latitude && Longitude == other.Longitude;

    public override bool Equals(object obj) => Equals(obj as CordinateVO);
    public override int GetHashCode() => HashCode.Combine(Latitude, Longitude);
    public override string ToString() => $"[{Longitude}, {Latitude}]";
}