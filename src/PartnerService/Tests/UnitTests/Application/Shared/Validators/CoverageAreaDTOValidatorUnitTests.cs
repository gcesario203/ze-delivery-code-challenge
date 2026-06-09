
using FluentAssertions;
using FluentValidation.TestHelper;
using PartnerService.Application.Shared.DataTransferObjects;
using PartnerService.Application.Shared.Validators;
using PartnerService.Tests.Fixtures;

namespace PartnerService.Tests.UnitTests.Application.Shared.Validators;

public class CoverageAreaDTOValidatorUnitTests
{
    private readonly CoverageAreaDTOValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenCoverageAreaIsValidRectangle()
    {
        var result = _validator.TestValidate(PartnerTestData.ValidCoverageArea);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldFail_WhenRingHasOnlyThreeDistinctVertices()
    {
        var closingPoint = new CordinateDTO { Latitude = -23.55052, Longitude = -46.633308 };

        var coverageArea = new CoverageAreaDTO
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

        var result = _validator.TestValidate(coverageArea);

        result.ShouldHaveValidationErrorFor(c => c.Cordinates);
    }
}
