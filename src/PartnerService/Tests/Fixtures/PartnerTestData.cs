
using PartnerService.Application.Partner.Commands.CreatePartner;
using PartnerService.Application.Shared.DataTransferObjects;

namespace PartnerService.Tests.Fixtures;

public static class PartnerTestData
{
    public static CordinateDTO ValidCoordinate => new()
    {
        Latitude = -23.55052,
        Longitude = -46.633308
    };

    public static AddressDTO ValidAddress => new()
    {
        Cordinates = ValidCoordinate
    };

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
                            closingPoint
                        ]
                    ]
                ]
            };
        }
    }

    public static CreatePartnerCommand CreateValidCommand(
        string document = "06369660000120",
        string tradingName = "Ze delivery",
        string ownerName = "Gabriel cesario")
        => new()
        {
            TradingName = tradingName,
            OwnerName = ownerName,
            Document = document,
            Address = ValidAddress,
            CoverageArea = ValidCoverageArea
        };
}
