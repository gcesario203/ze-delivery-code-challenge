
using GeolocalizationService.Application.PartnerGeolocation.UseCases;
using GeolocalizationService.Application.Shared.DataTransferObjects;

namespace GeolocalizationService.Tests.Fixtures;

public static class GeolocationTestData
{
    public static CordinateDTO SaoPauloCenter => new()
    {
        Latitude = -23.55052,
        Longitude = -46.633308
    };

    public static CordinateDTO SaoPauloNearby => new()
    {
        Latitude = -23.55100,
        Longitude = -46.63400
    };

    public static CordinateDTO RioDeJaneiro => new()
    {
        Latitude = -22.906847,
        Longitude = -43.172897
    };

    public static AddressDTO ValidAddress => new()
    {
        Cordinates = SaoPauloCenter
    };

    public static CoverageAreaDTO RioCoverageArea
    {
        get
        {
            var closingPoint = new CordinateDTO { Latitude = -22.906847, Longitude = -43.172897 };

            return new CoverageAreaDTO
            {
                Cordinates =
                [
                    [
                        [
                            closingPoint,
                            new CordinateDTO { Latitude = -22.916847, Longitude = -43.172897 },
                            new CordinateDTO { Latitude = -22.916847, Longitude = -43.182897 },
                            new CordinateDTO { Latitude = -22.906847, Longitude = -43.182897 },
                            closingPoint
                        ]
                    ]
                ]
            };
        }
    }

    public static CoverageAreaDTO ValidCoverageArea
    {
        get
        {
            var closingPoint = new CordinateDTO { Latitude = -23.55052, Longitude = -46.633308 };

            return new CoverageAreaDTO
            {
                Cordinates =
                [
                    [
                        [
                            closingPoint,
                            new CordinateDTO { Latitude = -23.56052, Longitude = -46.633308 },
                            new CordinateDTO { Latitude = -23.56052, Longitude = -46.643308 },
                            new CordinateDTO { Latitude = -23.55052, Longitude = -46.643308 },
                            closingPoint
                        ]
                    ]
                ]
            };
        }
    }

    public static CreatePartnerGeolocationInput CreateValidInput(Guid? id = null)
        => new()
        {
            Id = id ?? Guid.NewGuid(),
            Address = ValidAddress,
            CoverageArea = ValidCoverageArea
        };

    public static CreatePartnerGeolocationInput CreateRioInput(Guid? id = null)
        => new()
        {
            Id = id ?? Guid.NewGuid(),
            Address = new AddressDTO { Cordinates = RioDeJaneiro },
            CoverageArea = RioCoverageArea
        };

    public static CreatePartnerGeolocationInput CreateSaoPauloInputWithAddress(CordinateDTO address, Guid? id = null)
        => new()
        {
            Id = id ?? Guid.NewGuid(),
            Address = new AddressDTO { Cordinates = address },
            CoverageArea = ValidCoverageArea
        };

    public static UpdatePartnerGeolocationInput CreateValidUpdateInput(Guid id)
        => new()
        {
            Id = id,
            Address = new AddressDTO
            {
                Cordinates = new CordinateDTO { Latitude = -23.56100, Longitude = -46.64400 }
            },
            CoverageArea = ValidCoverageArea
        };
}
