
using FluentAssertions;
using GeolocalizationService.Domain.Shared.ValueObjects;

namespace GeolocalizationService.Tests.UnitTests.Domain.PartnerGeolocation;

public class CoverageAreaVOUnitTests
{
    private static CoverageAreaVO CreateSaoPauloCoverageArea()
    {
        return new CoverageAreaVO(
        [
            [
                [
                    new CordinateVO(-23.55052, -46.633308),
                    new CordinateVO(-23.56052, -46.633308),
                    new CordinateVO(-23.56052, -46.643308),
                    new CordinateVO(-23.55052, -46.643308),
                    new CordinateVO(-23.55052, -46.633308)
                ]
            ]
        ]);
    }

    [Fact]
    public void Contains_ShouldReturnTrue_WhenPointIsInsideCoverageArea()
    {
        var coverageArea = CreateSaoPauloCoverageArea();
        var point = new CordinateVO(-23.55100, -46.63400);

        coverageArea.Contains(point).Should().BeTrue();
    }

    [Fact]
    public void Contains_ShouldReturnFalse_WhenPointIsOutsideCoverageArea()
    {
        var coverageArea = CreateSaoPauloCoverageArea();
        var point = new CordinateVO(-22.906847, -43.172897);

        coverageArea.Contains(point).Should().BeFalse();
    }
}
