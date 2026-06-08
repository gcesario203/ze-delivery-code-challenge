

using GeolocalizationService.Domain.Shared.Enums;

namespace GeolocalizationService.Domain.Shared.ValueObjects;

public class AddressVO
{
    public CordinateVO Cordinates { get; private set; }
    public CordinateTypeVO Type { get; }

    public AddressVO(CordinateVO cordinates)
    {
        if(cordinates == null)
            throw new ArgumentException("Address coordinates cannot be null.", nameof(cordinates));

        Cordinates = cordinates;
        Type = CordinateTypeVO.Point;
    }
}