using GeolocalizationService.Domain.PartnerGeolocation.Entities;
using GeolocalizationService.Domain.Shared.ValueObjects;

namespace GeolocalizationService.Tests.UnitTests.Domain.PartnerGeolocation;

public class PartnerGeolocationEntityUnitTests
{
    private static AddressVO CreateValidAddress() =>
        new(new CordinateVO(10.0, 20.0));

    private static CoverageAreaVO CreateValidCoverageArea() =>
        new(
            new List<List<List<CordinateVO>>>
            {
                new()
                {
                    new List<CordinateVO>
                    {
                        new(10.0, 20.0),
                        new(15.0, 25.0),
                        new(20.0, 30.0),
                        new(10.0, 20.0),
                    },
                },
            });

    [Fact]
    public void Create_ShouldCreatePartnerGeolocationEntity()
    {
        var partnerId = Guid.NewGuid();
        var address = CreateValidAddress();
        var coverageArea = CreateValidCoverageArea();

        var geolocation = PartnerGeolocationEntity.Create(partnerId, address, coverageArea);

        Assert.NotNull(geolocation);
        Assert.Equal(partnerId, geolocation.PartnerId);
        Assert.Equal(address, geolocation.Address);
        Assert.Equal(coverageArea, geolocation.CoverageArea);
    }

    [Fact]
    public void Update_ShouldUpdatePartnerGeolocationEntity()
    {
        var partnerId = Guid.NewGuid();
        var geolocation = PartnerGeolocationEntity.Create(partnerId, CreateValidAddress(), CreateValidCoverageArea());

        var newAddress = new AddressVO(new CordinateVO(30.0, 40.0));
        var newCoverageArea = new CoverageAreaVO(
            new List<List<List<CordinateVO>>>
            {
                new()
                {
                    new List<CordinateVO>
                    {
                        new(30.0, 40.0),
                        new(35.0, 45.0),
                        new(40.0, 50.0),
                        new(30.0, 40.0),
                    },
                },
            });

        geolocation.Update(newAddress, newCoverageArea);

        Assert.Equal(newAddress, geolocation.Address);
        Assert.Equal(newCoverageArea, geolocation.CoverageArea);
    }

    [Fact]
    public void Constructor_ShouldCreatePartnerGeolocationEntity()
    {
        var partnerId = Guid.NewGuid();
        var address = CreateValidAddress();
        var coverageArea = CreateValidCoverageArea();

        var geolocation = new PartnerGeolocationEntity(partnerId, address, coverageArea);

        Assert.NotNull(geolocation);
        Assert.Equal(partnerId, geolocation.Id);
        Assert.Equal(partnerId, geolocation.PartnerId);
        Assert.Equal(address, geolocation.Address);
        Assert.Equal(coverageArea, geolocation.CoverageArea);
    }

    [Fact]
    public void Create_ShouldNotCreateWithEmptyPartnerId()
    {
        Assert.Throws<ArgumentException>(() =>
            PartnerGeolocationEntity.Create(Guid.Empty, CreateValidAddress(), CreateValidCoverageArea()));
    }

    [Fact]
    public void Create_ShouldNotCreateWithNullAddress()
    {
        Assert.Throws<ArgumentException>(() =>
            PartnerGeolocationEntity.Create(Guid.NewGuid(), null!, CreateValidCoverageArea()));
    }

    [Fact]
    public void Create_ShouldNotCreateWithNullCoverageArea()
    {
        Assert.Throws<ArgumentException>(() =>
            PartnerGeolocationEntity.Create(Guid.NewGuid(), CreateValidAddress(), null!));
    }

    [Fact]
    public void Create_ShouldNotCreateWithInvalidAddress()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new AddressVO(new CordinateVO(-100.0, -200.0)));
    }

    [Fact]
    public void Create_ShouldNotCreateWithInvalidCoverageArea()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new CoverageAreaVO(
            new List<List<List<CordinateVO>>>
            {
                new()
                {
                    new List<CordinateVO>
                    {
                        new(-100.0, -200.0),
                        new(150.0, 250.0),
                        new(200.0, 300.0),
                        new(100.0, 200.0),
                    },
                },
            }));
    }
}
